using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zlib;
using Klei;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using KSerialization;
using Newtonsoft.Json;
using ProcGen;
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

	private Cluster m_clusterLayout;

	private const string CorruptFileSuffix = "_";

	private const float SAVE_BUFFER_HEAD_ROOM = 0.1f;

	private bool mustRestartOnFail;

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

	public bool loadedFromSave { get; private set; }

	public static SaveLoader Instance { get; private set; }

	public Action<Cluster> OnWorldGenComplete { get; set; }

	public Cluster ClusterLayout => m_clusterLayout;

	public SaveGame.GameInfo GameInfo { get; private set; }

	public GameSpawnData cachedGSD { get; private set; }

	public WorldDetailSave clusterDetailSave { get; private set; }

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
			SimMessages.CreateDiseaseTable(Db.Get().Diseases);
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
		int i = 0;
		bool flag = WorldGen.CanLoad(WorldGen.GetSIMSaveFilename(i));
		if (flag && LoadFromWorldGen())
		{
			return;
		}
		DebugUtil.LogWarningArgs("Couldn't start new game with current world gen, moving file");
		if (flag)
		{
			KMonoBehaviour.isLoadingScene = true;
			for (; FileSystem.FileExists(WorldGen.GetSIMSaveFilename(i)); i++)
			{
				MoveCorruptFile(WorldGen.GetSIMSaveFilename(i));
			}
		}
		App.LoadScene("frontend");
	}

	private static void CompressContents(BinaryWriter fileWriter, byte[] uncompressed, int length)
	{
		using ZlibStream zlibStream = new ZlibStream(fileWriter.BaseStream, CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed);
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
		Sim.Save(writer, 0, 0);
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
		CustomGameSettings.Instance.LoadClusters();
		if (GameInfo.clusterId == null)
		{
			SaveGame.GameInfo gameInfo = GameInfo;
			if (!string.IsNullOrEmpty(saveFileRoot.clusterID))
			{
				gameInfo.clusterId = saveFileRoot.clusterID;
			}
			else
			{
				try
				{
					gameInfo.clusterId = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id;
				}
				catch
				{
					gameInfo.clusterId = WorldGenSettings.ClusterDefaultName;
					CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.ClusterLayout, gameInfo.clusterId);
				}
			}
			GameInfo = gameInfo;
		}
		Game.clusterId = GameInfo.clusterId;
		Game.LoadSettings(deserializer);
		GridSettings.Reset(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
		if (Application.isPlaying)
		{
			Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
		}
		Sim.SIM_Initialize(Sim.DLL_MessageHandler);
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		Sim.AllocateCells(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
		SimMessages.CreateDiseaseTable(Db.Get().Diseases);
		Sim.HandleMessage(SimMessageHashes.ClearUnoccupiedCells, 0, null);
		IReader reader2 = ((!saveFileRoot.streamed.ContainsKey("Sim")) ? reader : new FastReader(saveFileRoot.streamed["Sim"]));
		if (Sim.LoadWorld(reader2) != 0)
		{
			DebugUtil.LogWarningArgs("\n--- Error loading save ---\nSimDLL found bad data\n");
			Sim.Shutdown();
			return false;
		}
		Sim.Start();
		SceneInitializer.Instance.PostLoadPrefabs();
		mustRestartOnFail = true;
		if (!saveManager.Load(reader))
		{
			Sim.Shutdown();
			DebugUtil.LogWarningArgs("\n--- Error loading save ---\n");
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
		ClusterManager.Instance.InitializeWorldGrid();
		SimMessages.DefineWorldOffsets(ClusterManager.Instance.WorldContainers.Select(delegate(WorldContainer container)
		{
			SimMessages.WorldOffsetData result = default(SimMessages.WorldOffsetData);
			result.worldOffsetX = container.WorldOffset.x;
			result.worldOffsetY = container.WorldOffset.y;
			result.worldSizeX = container.WorldSize.x;
			result.worldSizeY = container.WorldSize.y;
			return result;
		}).ToList());
		return true;
	}

	public static string GetSavePrefix()
	{
		return System.IO.Path.Combine(Util.RootFolder(), "save_files/");
	}

	public static string GetCloudSavePrefix()
	{
		string path = System.IO.Path.Combine(Util.RootFolder(), "cloud_save_files/");
		string userID = GetUserID();
		if (string.IsNullOrEmpty(userID))
		{
			return null;
		}
		path = System.IO.Path.Combine(path, userID);
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
		string arg = System.IO.Path.ChangeExtension(filename, null);
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
		return System.IO.Path.GetFileName(filename);
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
		string text = System.IO.Path.Combine(GetSavePrefixAndCreateFolder(), "auto_save/");
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
		return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(activeSaveFilePath), "auto_save");
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
			text = System.IO.Path.Combine(GetSavePrefix(), Instance.GameInfo.baseName);
		}
		return text;
	}

	public static string GetActiveSaveFolder()
	{
		string activeSaveFilePath = GetActiveSaveFilePath();
		if (!string.IsNullOrEmpty(activeSaveFilePath))
		{
			return System.IO.Path.GetDirectoryName(activeSaveFilePath);
		}
		return null;
	}

	public static List<SaveFileEntry> GetSaveFiles(string save_dir, bool sort, SearchOption search = SearchOption.AllDirectories)
	{
		List<SaveFileEntry> list = new List<SaveFileEntry>();
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
			foreach (string text in files)
			{
				try
				{
					System.DateTime lastWriteTime = File.GetLastWriteTime(text);
					SaveFileEntry saveFileEntry = default(SaveFileEntry);
					saveFileEntry.path = text;
					saveFileEntry.timeStamp = lastWriteTime;
					SaveFileEntry item = saveFileEntry;
					list.Add(item);
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Problem reading file: " + text + "\n" + ex.ToString());
				}
			}
			if (sort)
			{
				list.Sort((SaveFileEntry x, SaveFileEntry y) => y.timeStamp.CompareTo(x.timeStamp));
				return list;
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

	public static List<SaveFileEntry> GetAllFiles(bool sort, SaveType type = SaveType.both)
	{
		switch (type)
		{
		case SaveType.local:
			return GetSaveFiles(GetSavePrefixAndCreateFolder(), sort);
		case SaveType.cloud:
			return GetSaveFiles(GetCloudSavePrefix(), sort);
		case SaveType.both:
		{
			List<SaveFileEntry> saveFiles = GetSaveFiles(GetSavePrefixAndCreateFolder(), sort: false);
			List<SaveFileEntry> saveFiles2 = GetSaveFiles(GetCloudSavePrefix(), sort: false);
			saveFiles.AddRange(saveFiles2);
			if (sort)
			{
				saveFiles.Sort((SaveFileEntry x, SaveFileEntry y) => y.timeStamp.CompareTo(x.timeStamp));
			}
			return saveFiles;
		}
		default:
			return new List<SaveFileEntry>();
		}
	}

	public static List<SaveFileEntry> GetAllColonyFiles(bool sort, SearchOption search = SearchOption.TopDirectoryOnly)
	{
		return GetSaveFiles(GetActiveSaveColonyFolder(), sort, search);
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
		List<SaveFileEntry> allFiles = GetAllFiles(sort: true);
		if (allFiles.Count == 0)
		{
			return null;
		}
		return allFiles[0].path;
	}

	public static string GetLatestSaveForCurrentDLC()
	{
		List<SaveFileEntry> allFiles = GetAllFiles(sort: true);
		for (int i = 0; i < allFiles.Count; i++)
		{
			Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(allFiles[i].path);
			if (fileInfo != null)
			{
				_ = fileInfo.first;
				SaveGame.GameInfo second = fileInfo.second;
				if (second.saveMajorVersion >= 7 && DlcManager.GetHighestActiveDlcId() == second.dlcId)
				{
					return allFiles[i].path;
				}
			}
		}
		return null;
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
		string directoryName = System.IO.Path.GetDirectoryName(filename);
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
			List<SaveFileEntry> saveFiles = GetSaveFiles(GetActiveAutoSavePath(), sort: true);
			List<string> list = new List<string>();
			foreach (SaveFileEntry item in saveFiles)
			{
				Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(item.path);
				if (fileInfo != null && SaveGame.GetSaveUniqueID(fileInfo.second) == Instance.GameInfo.colonyGuid.ToString())
				{
					list.Add(item.path);
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
				string text2 = System.IO.Path.ChangeExtension(text, ".png");
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
				KCrashReporter.MOST_RECENT_SAVEFILE = filename;
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
		return SaveGame.GetHeader(new FastReader(File.ReadAllBytes(filename)), out header, filename);
	}

	public bool Load(string filename)
	{
		SetActiveSaveFilePath(filename);
		try
		{
			KSerialization.Manager.Clear();
			byte[] array = File.ReadAllBytes(filename);
			IReader reader = new FastReader(array);
			GameInfo = SaveGame.GetHeader(reader, out var header, filename);
			DebugUtil.LogArgs(string.Format("Loading save file: {4}\n headerVersion:{0}, buildVersion:{1}, headerSize:{2}, IsCompressed:{3}", header.headerVersion, header.buildVersion, header.headerSize, header.IsCompressed, filename));
			DebugUtil.LogArgs(string.Format("GameInfo loaded from save header:\n  numberOfCycles:{0},\n  numberOfDuplicants:{1},\n  baseName:{2},\n  isAutoSave:{3},\n  originalSaveName:{4},\n  clusterId:{5},\n  worldTraits:{6},\n  colonyGuid:{7},\n  saveVersion:{8}.{9}", GameInfo.numberOfCycles, GameInfo.numberOfDuplicants, GameInfo.baseName, GameInfo.isAutoSave, GameInfo.originalSaveName, GameInfo.clusterId, (GameInfo.worldTraits != null && GameInfo.worldTraits.Length != 0) ? string.Join(", ", GameInfo.worldTraits) : "<i>none</i>", GameInfo.colonyGuid, GameInfo.saveMajorVersion, GameInfo.saveMinorVersion));
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
			KCrashReporter.MOST_RECENT_SAVEFILE = filename;
			if (GameInfo.isAutoSave && !string.IsNullOrEmpty(GameInfo.originalSaveName))
			{
				string text = null;
				string originalSaveFileName2 = GetOriginalSaveFileName(GameInfo.originalSaveName);
				if (IsSaveCloud(filename))
				{
					string cloudSavePrefix = GetCloudSavePrefix();
					text = ((cloudSavePrefix == null) ? System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename).Replace("auto_save", ""), GameInfo.baseName, originalSaveFileName2) : System.IO.Path.Combine(cloudSavePrefix, GameInfo.baseName, originalSaveFileName2));
				}
				else
				{
					text = System.IO.Path.Combine(GetSavePrefix(), GameInfo.baseName, originalSaveFileName2);
				}
				if (text != null)
				{
					SetActiveSaveFilePath(text);
				}
			}
		}
		catch (Exception ex)
		{
			DebugUtil.LogWarningArgs("\n--- Error loading save ---\n" + ex.Message + "\n" + ex.StackTrace);
			Sim.Shutdown();
			SetActiveSaveFilePath(null);
			return false;
		}
		Stats.Print();
		DebugUtil.LogArgs("Loaded", "[" + filename + "]");
		DebugUtil.LogArgs("World Seeds", "[" + clusterDetailSave.globalWorldSeed + "/" + clusterDetailSave.globalWorldLayoutSeed + "/" + clusterDetailSave.globalTerrainSeed + "/" + clusterDetailSave.globalNoiseSeed + "]");
		GC.Collect();
		return true;
	}

	public bool LoadFromWorldGen()
	{
		DebugUtil.LogArgs("Attempting to start a new game with current world gen");
		WorldGen.LoadSettings();
		m_clusterLayout = Cluster.Load();
		ListPool<SimSaveFileStructure, SaveLoader>.PooledList pooledList = ListPool<SimSaveFileStructure, SaveLoader>.Allocate();
		m_clusterLayout.LoadClusterLayoutSim(pooledList);
		SaveGame.GameInfo gameInfo = GameInfo;
		gameInfo.clusterId = m_clusterLayout.Id;
		gameInfo.colonyGuid = Guid.NewGuid();
		GameInfo = gameInfo;
		if (pooledList.Count != m_clusterLayout.worlds.Count)
		{
			Debug.LogError("Attempt failed. Failed to load all worlds.");
			pooledList.Recycle();
			return false;
		}
		GridSettings.Reset(m_clusterLayout.size.x, m_clusterLayout.size.y);
		if (Application.isPlaying)
		{
			Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
		}
		clusterDetailSave = new WorldDetailSave();
		foreach (SimSaveFileStructure item in pooledList)
		{
			clusterDetailSave.globalNoiseSeed = item.worldDetail.globalNoiseSeed;
			clusterDetailSave.globalTerrainSeed = item.worldDetail.globalTerrainSeed;
			clusterDetailSave.globalWorldLayoutSeed = item.worldDetail.globalWorldLayoutSeed;
			clusterDetailSave.globalWorldSeed = item.worldDetail.globalWorldSeed;
			Vector2 vector = Grid.CellToPos2D(Grid.PosToCell(new Vector2I(item.x, item.y)));
			foreach (WorldDetailSave.OverworldCell overworldCell in item.worldDetail.overworldCells)
			{
				for (int i = 0; i != overworldCell.poly.Vertices.Count; i++)
				{
					overworldCell.poly.Vertices[i] += vector;
				}
				overworldCell.poly.RefreshBounds();
			}
			clusterDetailSave.overworldCells.AddRange(item.worldDetail.overworldCells);
		}
		Sim.SIM_Initialize(Sim.DLL_MessageHandler);
		SimMessages.CreateSimElementsTable(ElementLoader.elements);
		Sim.AllocateCells(m_clusterLayout.size.x, m_clusterLayout.size.y);
		SimMessages.DefineWorldOffsets(m_clusterLayout.worlds.Select(delegate(WorldGen world)
		{
			SimMessages.WorldOffsetData result = default(SimMessages.WorldOffsetData);
			result.worldOffsetX = world.WorldOffset.x;
			result.worldOffsetY = world.WorldOffset.y;
			result.worldSizeX = world.WorldSize.x;
			result.worldSizeY = world.WorldSize.y;
			return result;
		}).ToList());
		SimMessages.CreateDiseaseTable(Db.Get().Diseases);
		Sim.HandleMessage(SimMessageHashes.ClearUnoccupiedCells, 0, null);
		try
		{
			foreach (SimSaveFileStructure item2 in pooledList)
			{
				FastReader reader = new FastReader(item2.Sim);
				if (Sim.Load(reader) != 0)
				{
					DebugUtil.LogWarningArgs("\n--- Error loading save ---\nSimDLL found bad data\n");
					Sim.Shutdown();
					pooledList.Recycle();
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("--- Error loading Sim FROM NEW WORLDGEN ---" + ex.Message + "\n" + ex.StackTrace);
			Sim.Shutdown();
			pooledList.Recycle();
			return false;
		}
		Debug.Log("Attempt success");
		Sim.Start();
		SceneInitializer.Instance.PostLoadPrefabs();
		SceneInitializer.Instance.NewSaveGamePrefab();
		cachedGSD = m_clusterLayout.currentWorld.SpawnData;
		OnWorldGenComplete.Signal(m_clusterLayout);
		ThreadedHttps<KleiMetrics>.Instance.StartNewGame();
		pooledList.Recycle();
		return true;
	}

	public void SetWorldDetail(WorldDetailSave worldDetail)
	{
		clusterDetailSave = worldDetail;
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
		Dictionary<Tag, float> allWorldsAccessibleAmounts = ClusterManager.Instance.GetAllWorldsAccessibleAmounts();
		List<WorldInventoryMetricsData> list = new List<WorldInventoryMetricsData>(allWorldsAccessibleAmounts.Count);
		foreach (KeyValuePair<Tag, float> item in allWorldsAccessibleAmounts)
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
