using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;
using Klei;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using KSerialization;
using Newtonsoft.Json;
using ProcGenGame;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SaveLoader")]
public class SaveLoader : KMonoBehaviour
{
	public class FlowUtilityNetworkInstance
	{
		public int id = -1;

		public SimHashes containedElement = SimHashes.Vacuum;

		public float containedMass;

		public float containedTemperature;
	}

	[SerializationConfig(KSerialization.MemberSerialization.OptOut)]
	public class FlowUtilityNetworkSaver : ISaveLoadable
	{
		public List<FlowUtilityNetworkInstance> gas;

		public List<FlowUtilityNetworkInstance> liquid;

		public FlowUtilityNetworkSaver()
		{
			gas = new List<FlowUtilityNetworkInstance>();
			liquid = new List<FlowUtilityNetworkInstance>();
		}
	}

	public struct SaveFileEntry
	{
		public string path;

		public System.DateTime timeStamp;
	}

	public enum SaveType
	{
		local,
		cloud,
		both
	}

	private struct MinionAttrFloatData
	{
		public string Name;

		public float Value;
	}

	private struct MinionMetricsData
	{
		public string Name;

		public List<MinionAttrFloatData> Modifiers;

		public float TotalExperienceGained;

		public List<string> Skills;
	}

	private struct SavedPrefabMetricsData
	{
		public string PrefabName;

		public int Count;
	}

	private struct WorldInventoryMetricsData
	{
		public string Name;

		public float Amount;
	}

	private struct DailyReportMetricsData
	{
		public string Name;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Net;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Positive;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Negative;
	}

	private struct PerformanceMeasurement
	{
		public string name;

		public float value;
	}

	[MyCmpGet]
	private GridSettings gridSettings;

	private bool saveFileCorrupt;

	private bool compressSaveData = true;

	private int lastUncompressedSize;

	public bool saveAsText;

	public const string MAINMENU_LEVELNAME = "launchscene";

	public const string FRONTEND_LEVELNAME = "frontend";

	public const string BACKEND_LEVELNAME = "backend";

	public const string SAVE_EXTENSION = ".sav";

	public const string AUTOSAVE_FOLDER = "auto_save";

	public const string CLOUDSAVE_FOLDER = "cloud_save_files";

	public const string SAVE_FOLDER = "save_files";

	public const int MAX_AUTOSAVE_FILES = 10;

	[NonSerialized]
	public SaveManager saveManager;

	private const string CorruptFileSuffix = "_";

	private const float SAVE_BUFFER_HEAD_ROOM = 0.1f;

	private bool mustRestartOnFail;

	public WorldGen worldGen;

	public const string METRIC_SAVED_PREFAB_KEY = "SavedPrefabs";

	public const string METRIC_IS_AUTO_SAVE_KEY = "IsAutoSave";

	public const string METRIC_WAS_DEBUG_EVER_USED = "WasDebugEverUsed";

	public const string METRIC_IS_SANDBOX_ENABLED = "IsSandboxEnabled";

	public const string METRIC_RESOURCES_ACCESSIBLE_KEY = "ResourcesAccessible";

	public const string METRIC_DAILY_REPORT_KEY = "DailyReport";

	public const string METRIC_MINION_METRICS_KEY = "MinionMetrics";

	public const string METRIC_CUSTOM_GAME_SETTINGS = "CustomGameSettings";

	public const string METRIC_PERFORMANCE_MEASUREMENTS = "PerformanceMeasurements";

	public const string METRIC_FRAME_TIME = "AverageFrameTime";

	private static bool force_infinity;

	public bool loadedFromSave
	{
		get;
		private set;
	}

	public static SaveLoader Instance
	{
		get;
		private set;
	}

	public System.Action OnWorldGenComplete
	{
		get;
		set;
	}

	public SaveGame.GameInfo GameInfo
	{
		get;
		private set;
	}

	public GameSpawnData cachedGSD
	{
		get;
		private set;
	}

	public WorldDetailSave worldDetailSave
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		saveManager = GetComponent<SaveManager>();
	}

	private void MoveCorruptFile(string filename)
	{
	}

	protected override void OnSpawn()
	{
		string activeSaveFilePath = GetActiveSaveFilePath();
		if (WorldGen.CanLoad(activeSaveFilePath))
		{
			Sim.SIM_Initialize(Sim.DLL_MessageHandler);
			SimMessages.CreateSimElementsTable(ElementLoader.elements);
			SimMessages.CreateDiseaseTable();
			loadedFromSave = true;
			loadedFromSave = Load(activeSaveFilePath);
			saveFileCorrupt = !loadedFromSave;
			if (!loadedFromSave)
			{
				SetActiveSaveFilePath(null);
				if (mustRestartOnFail)
				{
					MoveCorruptFile(activeSaveFilePath);
					Sim.Shutdown();
					App.LoadScene("frontend");
					return;
				}
			}
		}
		if (loadedFromSave)
		{
			return;
		}
		Sim.Shutdown();
		if (!string.IsNullOrEmpty(activeSaveFilePath))
		{
			DebugUtil.LogArgs("Couldn't load [" + activeSaveFilePath + "]");
		}
		if (saveFileCorrupt)
		{
			MoveCorruptFile(activeSaveFilePath);
		}
		bool flag = WorldGen.CanLoad(WorldGen.SIM_SAVE_FILENAME);
		if (!flag || !LoadFromWorldGen())
		{
			DebugUtil.LogWarningArgs("Couldn't start new game with current world gen, moving file");
			if (flag)
			{
				KMonoBehaviour.isLoadingScene = true;
				MoveCorruptFile(WorldGen.SIM_SAVE_FILENAME);
			}
			App.LoadScene("frontend");
		}
	}

	private static void CompressContents(BinaryWriter fileWriter, byte[] uncompressed, int length)
	{
		using ZlibStream zlibStream = new ZlibStream(fileWriter.BaseStream, CompressionMode.Compress, CompressionLevel.BestSpeed);
		zlibStream.Write(uncompressed, 0, length);
		zlibStream.Flush();
	}

	private byte[] FloatToBytes(float[] floats)
	{
		byte[] array = new byte[floats.Length * 4];
		Buffer.BlockCopy(floats, 0, array, 0, array.Length);
		return array;
	}

	private static byte[] DecompressContents(byte[] compressed)
	{
		return ZlibStream.UncompressBuffer(compressed);
	}

	private float[] BytesToFloat(byte[] bytes)
	{
		float[] array = new float[bytes.Length / 4];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		return array;
	}

	private SaveFileRoot PrepSaveFile()
	{
		SaveFileRoot saveFileRoot = new SaveFileRoot();
		saveFileRoot.WidthInCells = Grid.WidthInCells;
		saveFileRoot.HeightInCells = Grid.HeightInCells;
		saveFileRoot.streamed["GridVisible"] = Grid.Visible;
		saveFileRoot.streamed["GridSpawnable"] = Grid.Spawnable;
		saveFileRoot.streamed["GridDamage"] = FloatToBytes(Grid.Damage);
		Global.Instance.modManager.SendMetricsEvent();
		saveFileRoot.active_mods = new List<Label>();
		foreach (Mod mod in Global.Instance.modManager.mods)
		{
			if (mod.IsEnabledForActiveDlc())
			{
				saveFileRoot.active_mods.Add(mod.label);
			}
		}
		using MemoryStream memoryStream = new MemoryStream();
		using (BinaryWriter writer = new BinaryWriter(memoryStream))
		{
			Camera.main.transform.parent.GetComponent<CameraController>().Save(writer);
		}
		saveFileRoot.streamed["Camera"] = memoryStream.ToArray();
		return saveFileRoot;
	}

	private void Save(BinaryWriter writer)
	{
		writer.WriteKleiString("world");
		Serializer.Serialize(PrepSaveFile(), writer);
		Game.SaveSettings(writer);
		Sim.Save(writer);
		saveManager.Save(writer);
		Game.Instance.Save(writer);
	}

	private bool Load(IReader reader)
	{
		Debug.Assert(reader.ReadKleiString() == "world");
		Deserializer deserializer = new Deserializer(reader);
		SaveFileRoot saveFileRoot = new SaveFileRoot();
		deserializer.Deserialize(saveFileRoot);
		if ((GameInfo.saveMajorVersion == 7 || GameInfo.saveMinorVersion < 8) && saveFileRoot.requiredMods != null)
		{
			saveFileRoot.active_mods = new List<Label>();
			foreach (ModInfo requiredMod in saveFileRoot.requiredMods)
			{
				saveFileRoot.active_mods.Add(new Label
				{
					id = requiredMod.assetID,
					version = (long)requiredMod.lastModifiedTime,
					distribution_platform = Label.DistributionPlatform.Steam,
					title = requiredMod.description
				});
			}
			saveFileRoot.requiredMods.Clear();
		}
		KMod.Manager modManager = Global.Instance.modManager;
		modManager.Load(Content.LayerableFiles);
		if (!modManager.MatchFootprint(saveFileRoot.active_mods, Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation))
		{
			DebugUtil.LogWarningArgs("Mod footprint of save file doesn't match current mod configuration");
		}
		Global.Instance.modManager.SendMetricsEvent();
		WorldGen.LoadSettings();
		CustomGameSettings.Instance.LoadWorlds();
		if (GameInfo.worldID == null)
		{
			SaveGame.GameInfo gameInfo = GameInfo;
			if (!string.IsNullOrEmpty(saveFileRoot.worldID))
			{
				gameInfo.worldID = saveFileRoot.worldID;
			}
			else
			{
				try
				{
					gameInfo.worldID = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id;
				}
				catch
				{
					gameInfo.worldID = "worlds/SandstoneDefault";
				}
			}
			GameInfo = gameInfo;
		}
		if (GameInfo.worldTraits == null)
		{
			SaveGame.GameInfo gameInfo2 = GameInfo;
			gameInfo2.worldTraits = new string[0];
			GameInfo = gameInfo2;
		}
		worldGen = new WorldGen(GameInfo.worldID, new List<string>(GameInfo.worldTraits), assertMissingTraits: false);
		Game.LoadSettings(deserializer);
		GridSettings.Reset(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
		Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
		Sim.SIM_Initialize(Sim.DLL_MessageHandler);
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		SimMessages.CreateDiseaseTable();
		IReader reader2 = ((!saveFileRoot.streamed.ContainsKey("Sim")) ? reader : new FastReader(saveFileRoot.streamed["Sim"]));
		if (Sim.Load(reader2) != 0)
		{
			DebugUtil.LogWarningArgs("--- Error loading save ---\nSimDLL found bad data\n");
			Sim.Shutdown();
			return false;
		}
		SceneInitializer.Instance.PostLoadPrefabs();
		mustRestartOnFail = true;
		if (!saveManager.Load(reader))
		{
			Sim.Shutdown();
			DebugUtil.LogWarningArgs("--- Error loading save ---\n");
			SetActiveSaveFilePath(null);
			return false;
		}
		Grid.Visible = saveFileRoot.streamed["GridVisible"];
		if (saveFileRoot.streamed.ContainsKey("GridSpawnable"))
		{
			Grid.Spawnable = saveFileRoot.streamed["GridSpawnable"];
		}
		Grid.Damage = BytesToFloat(saveFileRoot.streamed["GridDamage"]);
		Game.Instance.Load(deserializer);
		CameraSaveData.Load(new FastReader(saveFileRoot.streamed["Camera"]));
		return true;
	}

	public static string GetSavePrefix()
	{
		return Path.Combine(Util.RootFolder(), "save_files/");
	}

	public static string GetCloudSavePrefix()
	{
		string path = Path.Combine(Util.RootFolder(), "cloud_save_files/");
		string userID = GetUserID();
		if (string.IsNullOrEmpty(userID))
		{
			return null;
		}
		path = Path.Combine(path, userID);
		if (!System.IO.Directory.Exists(path))
		{
			System.IO.Directory.CreateDirectory(path);
		}
		return path;
	}

	public static string GetSavePrefixAndCreateFolder()
	{
		string savePrefix = GetSavePrefix();
		if (!System.IO.Directory.Exists(savePrefix))
		{
			System.IO.Directory.CreateDirectory(savePrefix);
		}
		return savePrefix;
	}

	public static string GetUserID()
	{
		return DistributionPlatform.Inst.LocalUser?.Id.ToString();
	}

	public static string GetNextUsableSavePath(string filename)
	{
		int num = 0;
		string arg = Path.ChangeExtension(filename, null);
		while (File.Exists(filename))
		{
			filename = SaveScreen.GetValidSaveFilename($"{arg} ({num})");
			num++;
		}
		return filename;
	}

	public static string GetOriginalSaveFileName(string filename)
	{
		if (!filename.Contains("/") && !filename.Contains("\\"))
		{
			return filename;
		}
		filename.Replace('\\', '/');
		return Path.GetFileName(filename);
	}

	public static bool IsSaveAuto(string filename)
	{
		filename = filename.Replace('\\', '/');
		return filename.Contains("/auto_save/");
	}

	public static bool IsSaveLocal(string filename)
	{
		filename = filename.Replace('\\', '/');
		return filename.Contains("/save_files/");
	}

	public static bool IsSaveCloud(string filename)
	{
		filename = filename.Replace('\\', '/');
		return filename.Contains("/cloud_save_files/");
	}

	public static string GetAutoSavePrefix()
	{
		string text = Path.Combine(GetSavePrefixAndCreateFolder(), "auto_save/");
		if (!System.IO.Directory.Exists(text))
		{
			System.IO.Directory.CreateDirectory(text);
		}
		return text;
	}

	public static void SetActiveSaveFilePath(string path)
	{
		KPlayerPrefs.SetString("SaveFilenameKey/", path);
	}

	public static string GetActiveSaveFilePath()
	{
		return KPlayerPrefs.GetString("SaveFilenameKey/");
	}

	public static string GetActiveAutoSavePath()
	{
		string activeSaveFilePath = GetActiveSaveFilePath();
		if (activeSaveFilePath == null)
		{
			return GetAutoSavePrefix();
		}
		return Path.Combine(Path.GetDirectoryName(activeSaveFilePath), "auto_save");
	}

	public static string GetAutosaveFilePath()
	{
		return GetAutoSavePrefix() + "AutoSave Cycle 1.sav";
	}

	public static string GetActiveSaveColonyFolder()
	{
		string text = GetActiveSaveFolder();
		if (text == null)
		{
			text = Path.Combine(GetSavePrefix(), Instance.GameInfo.baseName);
		}
		return text;
	}

	public static string GetActiveSaveFolder()
	{
		string activeSaveFilePath = GetActiveSaveFilePath();
		if (!string.IsNullOrEmpty(activeSaveFilePath))
		{
			return Path.GetDirectoryName(activeSaveFilePath);
		}
		return null;
	}

	public static List<string> GetSaveFiles(string save_dir, SearchOption search = SearchOption.AllDirectories)
	{
		List<string> list = new List<string>();
		if (string.IsNullOrEmpty(save_dir))
		{
			return list;
		}
		try
		{
			if (!System.IO.Directory.Exists(save_dir))
			{
				System.IO.Directory.CreateDirectory(save_dir);
			}
			string[] files = System.IO.Directory.GetFiles(save_dir, "*.sav", search);
			List<SaveFileEntry> list2 = new List<SaveFileEntry>();
			string[] array = files;
			foreach (string text in array)
			{
				try
				{
					System.DateTime lastWriteTime = File.GetLastWriteTime(text);
					SaveFileEntry saveFileEntry = default(SaveFileEntry);
					saveFileEntry.path = text;
					saveFileEntry.timeStamp = lastWriteTime;
					SaveFileEntry item = saveFileEntry;
					list2.Add(item);
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Problem reading file: " + text + "\n" + ex.ToString());
				}
			}
			list2.Sort((SaveFileEntry x, SaveFileEntry y) => y.timeStamp.CompareTo(x.timeStamp));
			foreach (SaveFileEntry item2 in list2)
			{
				list.Add(item2.path);
			}
			return list;
		}
		catch (Exception ex2)
		{
			string text2 = null;
			if (ex2 is UnauthorizedAccessException)
			{
				text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, save_dir);
			}
			else if (ex2 is IOException)
			{
				text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, save_dir);
			}
			if (text2 != null)
			{
				GameObject parent = ((FrontEndManager.Instance == null) ? GameScreenManager.Instance.ssOverlayCanvas : FrontEndManager.Instance.gameObject);
				Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent, force_active: true).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(text2, null, null);
				return list;
			}
			throw ex2;
		}
	}

	public static List<string> GetAllFiles(SaveType type = SaveType.both)
	{
		switch (type)
		{
		case SaveType.local:
			return GetSaveFiles(GetSavePrefixAndCreateFolder());
		case SaveType.cloud:
			return GetSaveFiles(GetCloudSavePrefix());
		case SaveType.both:
		{
			List<string> saveFiles = GetSaveFiles(GetSavePrefixAndCreateFolder());
			List<string> saveFiles2 = GetSaveFiles(GetCloudSavePrefix());
			for (int i = 0; i < saveFiles2.Count; i++)
			{
				saveFiles.Add(saveFiles2[i]);
			}
			saveFiles.Sort(delegate(string x, string y)
			{
				System.DateTime lastWriteTime = File.GetLastWriteTime(x);
				return File.GetLastWriteTime(y).CompareTo(lastWriteTime);
			});
			return saveFiles;
		}
		default:
			return new List<string>();
		}
	}

	public static List<string> GetAllColonyFiles(SearchOption search = SearchOption.TopDirectoryOnly)
	{
		return GetSaveFiles(GetActiveSaveColonyFolder(), search);
	}

	public static bool GetCloudSavesDefault()
	{
		if (!(GetCloudSavesDefaultPref() == "Disabled"))
		{
			return true;
		}
		return false;
	}

	public static string GetCloudSavesDefaultPref()
	{
		string text = KPlayerPrefs.GetString("SavesDefaultToCloud", "Enabled");
		if (text != "Enabled" && text != "Disabled")
		{
			text = "Enabled";
		}
		return text;
	}

	public static void SetCloudSavesDefault(bool value)
	{
		SetCloudSavesDefaultPref(value ? "Enabled" : "Disabled");
	}

	public static void SetCloudSavesDefaultPref(string pref)
	{
		if (pref != "Enabled" && pref != "Disabled")
		{
			Debug.LogWarning("Ignoring cloud saves default pref `" + pref + "` as it's not valid, expected `Enabled` or `Disabled`");
		}
		else
		{
			KPlayerPrefs.SetString("SavesDefaultToCloud", pref);
		}
	}

	public static bool GetCloudSavesAvailable()
	{
		if (string.IsNullOrEmpty(GetUserID()))
		{
			return false;
		}
		if (GetCloudSavePrefix() == null)
		{
			return false;
		}
		return true;
	}

	public static string GetLatestSaveFile()
	{
		List<string> allFiles = GetAllFiles();
		if (allFiles.Count == 0)
		{
			return null;
		}
		return allFiles[0];
	}

	public void InitialSave()
	{
		string text = GetActiveSaveFilePath();
		if (string.IsNullOrEmpty(text))
		{
			text = GetAutosaveFilePath();
		}
		else if (!text.Contains(".sav"))
		{
			text += ".sav";
		}
		Save(text);
	}

	public string Save(string filename, bool isAutoSave = false, bool updateSavePointer = true)
	{
		KSerialization.Manager.Clear();
		string directoryName = Path.GetDirectoryName(filename);
		try
		{
			if (directoryName != null && !System.IO.Directory.Exists(directoryName))
			{
				System.IO.Directory.CreateDirectory(directoryName);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Problem creating save folder for " + filename + "!\n" + ex.ToString());
		}
		ReportSaveMetrics(isAutoSave);
		RetireColonyUtility.SaveColonySummaryData();
		if (isAutoSave && !GenericGameSettings.instance.keepAllAutosaves)
		{
			List<string> saveFiles = GetSaveFiles(GetActiveAutoSavePath());
			List<string> list = new List<string>();
			foreach (string item in saveFiles)
			{
				Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(item);
				if (fileInfo != null && SaveGame.GetSaveUniqueID(fileInfo.second) == Instance.GameInfo.colonyGuid.ToString())
				{
					list.Add(item);
				}
			}
			for (int num = list.Count - 1; num >= 9; num--)
			{
				string text = list[num];
				try
				{
					Debug.Log("Deleting old autosave: " + text);
					File.Delete(text);
				}
				catch (Exception ex2)
				{
					Debug.LogWarning("Problem deleting autosave: " + text + "\n" + ex2.ToString());
				}
				string text2 = Path.ChangeExtension(text, ".png");
				try
				{
					if (File.Exists(text2))
					{
						File.Delete(text2);
					}
				}
				catch (Exception ex3)
				{
					Debug.LogWarning("Problem deleting autosave screenshot: " + text2 + "\n" + ex3.ToString());
				}
			}
		}
		using (MemoryStream memoryStream = new MemoryStream((int)((float)lastUncompressedSize * 1.1f)))
		{
			using BinaryWriter writer = new BinaryWriter(memoryStream);
			Save(writer);
			lastUncompressedSize = (int)memoryStream.Length;
			try
			{
				using BinaryWriter binaryWriter = new BinaryWriter(File.Open(filename, FileMode.Create));
				SaveGame.Header header;
				byte[] saveHeader = SaveGame.Instance.GetSaveHeader(isAutoSave, compressSaveData, out header);
				binaryWriter.Write(header.buildVersion);
				binaryWriter.Write(header.headerSize);
				binaryWriter.Write(header.headerVersion);
				binaryWriter.Write(header.compression);
				binaryWriter.Write(saveHeader);
				KSerialization.Manager.SerializeDirectory(binaryWriter);
				if (compressSaveData)
				{
					CompressContents(binaryWriter, memoryStream.GetBuffer(), (int)memoryStream.Length);
				}
				else
				{
					binaryWriter.Write(memoryStream.ToArray());
				}
				Stats.Print();
			}
			catch (Exception ex4)
			{
				if (ex4 is UnauthorizedAccessException)
				{
					DebugUtil.LogArgs("UnauthorizedAccessException for " + filename);
					((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject)).PopupConfirmDialog(string.Format(UI.CRASHSCREEN.SAVEFAILED, "Unauthorized Access Exception"), null, null);
					return GetActiveSaveFilePath();
				}
				if (ex4 is IOException)
				{
					DebugUtil.LogArgs("IOException (probably out of disk space) for " + filename);
					((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject)).PopupConfirmDialog(string.Format(UI.CRASHSCREEN.SAVEFAILED, "IOException. You may not have enough free space!"), null, null);
					return GetActiveSaveFilePath();
				}
				throw ex4;
			}
		}
		if (updateSavePointer)
		{
			SetActiveSaveFilePath(filename);
		}
		Game.Instance.timelapser.SaveColonyPreview(filename);
		DebugUtil.LogArgs("Saved to", "[" + filename + "]");
		GC.Collect();
		return filename;
	}

	public static SaveGame.GameInfo LoadHeader(string filename, out SaveGame.Header header)
	{
		return SaveGame.GetHeader(new FastReader(File.ReadAllBytes(filename)), out header);
	}

	public bool Load(string filename)
	{
		SetActiveSaveFilePath(filename);
		try
		{
			KSerialization.Manager.Clear();
			byte[] array = File.ReadAllBytes(filename);
			IReader reader = new FastReader(array);
			GameInfo = SaveGame.GetHeader(reader, out var header);
			DebugUtil.LogArgs(string.Format("Loading save file: {4}\n headerVersion:{0}, buildVersion:{1}, headerSize:{2}, IsCompressed:{3}", header.headerVersion, header.buildVersion, header.headerSize, header.IsCompressed, filename));
			DebugUtil.LogArgs(string.Format("GameInfo loaded from save header:\n  numberOfCycles:{0},\n  numberOfDuplicants:{1},\n  baseName:{2},\n  isAutoSave:{3},\n  originalSaveName:{4},\n  worldID:{5},\n  worldTraits:{6},\n  colonyGuid:{7},\n  saveVersion:{8}.{9}", GameInfo.numberOfCycles, GameInfo.numberOfDuplicants, GameInfo.baseName, GameInfo.isAutoSave, GameInfo.originalSaveName, GameInfo.worldID, (GameInfo.worldTraits != null && GameInfo.worldTraits.Length != 0) ? string.Join(", ", GameInfo.worldTraits) : "<i>none</i>", GameInfo.colonyGuid, GameInfo.saveMajorVersion, GameInfo.saveMinorVersion));
			string originalSaveName = GameInfo.originalSaveName;
			if (originalSaveName.Contains("/") || originalSaveName.Contains("\\"))
			{
				string originalSaveFileName = GetOriginalSaveFileName(originalSaveName);
				SaveGame.GameInfo gameInfo = GameInfo;
				gameInfo.originalSaveName = originalSaveFileName;
				GameInfo = gameInfo;
				Debug.Log("Migration / Save originalSaveName updated from: `" + originalSaveName + "` => `" + GameInfo.originalSaveName + "`");
			}
			if (GameInfo.saveMajorVersion == 7 && GameInfo.saveMinorVersion < 4)
			{
				Helper.SetTypeInfoMask((SerializationTypeInfo)191);
			}
			KSerialization.Manager.DeserializeDirectory(reader);
			if (header.IsCompressed)
			{
				int num = array.Length - reader.Position;
				byte[] array2 = new byte[num];
				Array.Copy(array, reader.Position, array2, 0, num);
				byte[] array3 = DecompressContents(array2);
				lastUncompressedSize = array3.Length;
				IReader reader2 = new FastReader(array3);
				Load(reader2);
			}
			else
			{
				lastUncompressedSize = array.Length;
				Load(reader);
			}
			if (GameInfo.isAutoSave && !string.IsNullOrEmpty(GameInfo.originalSaveName))
			{
				string text = null;
				string originalSaveFileName2 = GetOriginalSaveFileName(GameInfo.originalSaveName);
				if (IsSaveCloud(filename))
				{
					string cloudSavePrefix = GetCloudSavePrefix();
					text = ((cloudSavePrefix == null) ? Path.Combine(Path.GetDirectoryName(filename).Replace("auto_save", ""), GameInfo.baseName, originalSaveFileName2) : Path.Combine(cloudSavePrefix, GameInfo.baseName, originalSaveFileName2));
				}
				else
				{
					text = Path.Combine(GetSavePrefix(), GameInfo.baseName, originalSaveFileName2);
				}
				if (text != null)
				{
					SetActiveSaveFilePath(text);
				}
			}
		}
		catch (Exception ex)
		{
			DebugUtil.LogWarningArgs("--- Error loading save ---\n" + ex.Message + "\n" + ex.StackTrace);
			Sim.Shutdown();
			SetActiveSaveFilePath(null);
			return false;
		}
		Stats.Print();
		DebugUtil.LogArgs("Loaded", "[" + filename + "]");
		DebugUtil.LogArgs("World Seeds", "[" + worldDetailSave.globalWorldSeed + "/" + worldDetailSave.globalWorldLayoutSeed + "/" + worldDetailSave.globalTerrainSeed + "/" + worldDetailSave.globalNoiseSeed + "]");
		GC.Collect();
		return true;
	}

	public bool LoadFromWorldGen()
	{
		DebugUtil.LogArgs("Attempting to start a new game with current world gen");
		WorldGen.LoadSettings();
		WorldGen.LoadWorldGen(out var worldID, out var traitIDs, out var data, out var stats);
		worldGen = new WorldGen(worldID, traitIDs, data, stats, assertMissingTraits: true);
		SaveGame.GameInfo gameInfo = GameInfo;
		gameInfo.worldID = worldID;
		gameInfo.worldTraits = traitIDs.ToArray();
		gameInfo.colonyGuid = Guid.NewGuid();
		GameInfo = gameInfo;
		SimSaveFileStructure simSaveFileStructure = WorldGen.LoadWorldGenSim();
		if (simSaveFileStructure == null)
		{
			Debug.LogError("Attempt failed");
			return false;
		}
		worldDetailSave = simSaveFileStructure.worldDetail;
		if (worldDetailSave == null)
		{
			Debug.LogError("Detail is null");
		}
		Instance.SetWorldDetail(worldDetailSave);
		GridSettings.Reset(simSaveFileStructure.WidthInCells, simSaveFileStructure.HeightInCells);
		Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
		Sim.SIM_Initialize(Sim.DLL_MessageHandler);
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		SimMessages.CreateDiseaseTable();
		try
		{
			FastReader reader = new FastReader(simSaveFileStructure.Sim);
			if (Sim.Load(reader) != 0)
			{
				DebugUtil.LogWarningArgs("--- Error loading save ---\nSimDLL found bad data\n");
				Sim.Shutdown();
				return false;
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("--- Error loading Sim FROM NEW WORLDGEN ---" + ex.Message + "\n" + ex.StackTrace);
			Sim.Shutdown();
			return false;
		}
		Debug.Log("Attempt success");
		SceneInitializer.Instance.PostLoadPrefabs();
		SceneInitializer.Instance.NewSaveGamePrefab();
		cachedGSD = worldGen.SpawnData;
		OnWorldGenComplete.Signal();
		ThreadedHttps<KleiMetrics>.Instance.StartNewGame();
		return true;
	}

	public void SetWorldDetail(WorldDetailSave worldDetail)
	{
		worldDetailSave = worldDetail;
	}

	private void ReportSaveMetrics(bool is_auto_save)
	{
		if (ThreadedHttps<KleiMetrics>.Instance != null && ThreadedHttps<KleiMetrics>.Instance.enabled && !(saveManager == null))
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary[GameClock.NewCycleKey] = GameClock.Instance.GetCycle() + 1;
			dictionary["IsAutoSave"] = is_auto_save;
			dictionary["SavedPrefabs"] = GetSavedPrefabMetrics();
			dictionary["ResourcesAccessible"] = GetWorldInventoryMetrics();
			dictionary["MinionMetrics"] = GetMinionMetrics();
			if (is_auto_save)
			{
				dictionary["DailyReport"] = GetDailyReportMetrics();
				dictionary["PerformanceMeasurements"] = GetPerformanceMeasurements();
				dictionary["AverageFrameTime"] = GetFrameTime();
			}
			dictionary["CustomGameSettings"] = CustomGameSettings.Instance.GetSettingsForMetrics();
			ThreadedHttps<KleiMetrics>.Instance.SendEvent(dictionary, "ReportSaveMetrics");
		}
	}

	private List<MinionMetricsData> GetMinionMetrics()
	{
		List<MinionMetricsData> list = new List<MinionMetricsData>();
		foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
		{
			if (item == null)
			{
				continue;
			}
			Amounts amounts = item.gameObject.GetComponent<Modifiers>().amounts;
			List<MinionAttrFloatData> list2 = new List<MinionAttrFloatData>(amounts.Count);
			foreach (AmountInstance item2 in amounts)
			{
				float value = item2.value;
				if (!float.IsNaN(value) && !float.IsInfinity(value))
				{
					list2.Add(new MinionAttrFloatData
					{
						Name = item2.modifier.Id,
						Value = item2.value
					});
				}
			}
			MinionResume component = item.gameObject.GetComponent<MinionResume>();
			float totalExperienceGained = component.TotalExperienceGained;
			List<string> list3 = new List<string>();
			foreach (KeyValuePair<string, bool> item3 in component.MasteryBySkillID)
			{
				if (item3.Value)
				{
					list3.Add(item3.Key);
				}
			}
			list.Add(new MinionMetricsData
			{
				Name = item.name,
				Modifiers = list2,
				TotalExperienceGained = totalExperienceGained,
				Skills = list3
			});
		}
		return list;
	}

	private List<SavedPrefabMetricsData> GetSavedPrefabMetrics()
	{
		Dictionary<Tag, List<SaveLoadRoot>> lists = saveManager.GetLists();
		List<SavedPrefabMetricsData> list = new List<SavedPrefabMetricsData>(lists.Count);
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> item in lists)
		{
			Tag key = item.Key;
			List<SaveLoadRoot> value = item.Value;
			if (value.Count > 0)
			{
				list.Add(new SavedPrefabMetricsData
				{
					PrefabName = key.ToString(),
					Count = value.Count
				});
			}
		}
		return list;
	}

	private List<WorldInventoryMetricsData> GetWorldInventoryMetrics()
	{
		Dictionary<Tag, float> accessibleAmounts = WorldInventory.Instance.GetAccessibleAmounts();
		List<WorldInventoryMetricsData> list = new List<WorldInventoryMetricsData>(accessibleAmounts.Count);
		foreach (KeyValuePair<Tag, float> item in accessibleAmounts)
		{
			float value = item.Value;
			if (!float.IsInfinity(value) && !float.IsNaN(value))
			{
				list.Add(new WorldInventoryMetricsData
				{
					Name = item.Key.ToString(),
					Amount = value
				});
			}
		}
		return list;
	}

	private List<DailyReportMetricsData> GetDailyReportMetrics()
	{
		List<DailyReportMetricsData> list = new List<DailyReportMetricsData>();
		int cycle = GameClock.Instance.GetCycle();
		ReportManager.DailyReport dailyReport = ReportManager.Instance.FindReport(cycle);
		if (dailyReport != null)
		{
			foreach (ReportManager.ReportEntry reportEntry in dailyReport.reportEntries)
			{
				DailyReportMetricsData item = default(DailyReportMetricsData);
				item.Name = reportEntry.reportType.ToString();
				if (!float.IsInfinity(reportEntry.Net) && !float.IsNaN(reportEntry.Net))
				{
					item.Net = reportEntry.Net;
				}
				if (force_infinity)
				{
					item.Net = null;
				}
				if (!float.IsInfinity(reportEntry.Positive) && !float.IsNaN(reportEntry.Positive))
				{
					item.Positive = reportEntry.Positive;
				}
				if (!float.IsInfinity(reportEntry.Negative) && !float.IsNaN(reportEntry.Negative))
				{
					item.Negative = reportEntry.Negative;
				}
				list.Add(item);
			}
			list.Add(new DailyReportMetricsData
			{
				Name = "MinionCount",
				Net = Components.LiveMinionIdentities.Count,
				Positive = 0f,
				Negative = 0f
			});
		}
		return list;
	}

	private List<PerformanceMeasurement> GetPerformanceMeasurements()
	{
		List<PerformanceMeasurement> list = new List<PerformanceMeasurement>();
		if (Global.Instance != null)
		{
			PerformanceMonitor component = Global.Instance.GetComponent<PerformanceMonitor>();
			PerformanceMeasurement item = new PerformanceMeasurement
			{
				name = "FramesAbove30",
				value = component.NumFramesAbove30
			};
			list.Add(item);
			item = new PerformanceMeasurement
			{
				name = "FramesBelow30",
				value = component.NumFramesBelow30
			};
			list.Add(item);
			component.Reset();
		}
		return list;
	}

	private float GetFrameTime()
	{
		PerformanceMonitor component = Global.Instance.GetComponent<PerformanceMonitor>();
		DebugUtil.LogArgs("Average frame time:", 1f / component.FPS);
		return 1f / component.FPS;
	}
}
