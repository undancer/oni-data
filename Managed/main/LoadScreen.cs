using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : KModalScreen
{
	private struct SaveGameFileDetails
	{
		public string BaseName;

		public string FileName;

		public string UniqueID;

		public System.DateTime FileDate;

		public SaveGame.Header FileHeader;

		public SaveGame.GameInfo FileInfo;

		public long Size;
	}

	private class SelectedSave
	{
		public string filename;

		public KButton button;
	}

	private const int MAX_CLOUD_TUTORIALS = 5;

	private const string CLOUD_TUTORIAL_KEY = "LoadScreenCloudTutorialTimes";

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private GameObject saveButtonRoot;

	[SerializeField]
	private GameObject colonyListRoot;

	[SerializeField]
	private GameObject colonyViewRoot;

	[SerializeField]
	private HierarchyReferences migrationPanelRefs;

	[SerializeField]
	private HierarchyReferences saveButtonPrefab;

	[Space]
	[SerializeField]
	private KButton colonyCloudButton;

	[SerializeField]
	private KButton colonyLocalButton;

	[SerializeField]
	private KButton colonyInfoButton;

	[SerializeField]
	private Sprite localToCloudSprite;

	[SerializeField]
	private Sprite cloudToLocalSprite;

	[SerializeField]
	private Sprite errorSprite;

	[SerializeField]
	private Sprite infoSprite;

	[SerializeField]
	private Bouncer cloudTutorialBouncer;

	public bool requireConfirmation = true;

	private SelectedSave selectedSave;

	private List<SaveGameFileDetails> currentColony;

	private UIPool<HierarchyReferences> colonyListPool;

	private ConfirmDialogScreen confirmScreen;

	private InfoDialogScreen infoScreen;

	private InfoDialogScreen errorInfoScreen;

	private ConfirmDialogScreen errorScreen;

	private InspectSaveScreen inspectScreenInstance;

	public static LoadScreen Instance
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
		Debug.Assert(Instance == null);
		Instance = this;
		base.OnPrefabInit();
		colonyListPool = new UIPool<HierarchyReferences>(saveButtonPrefab);
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Pause(playSound: false);
		}
		if (closeButton != null)
		{
			closeButton.onClick += delegate
			{
				Deactivate();
			};
		}
		if (colonyCloudButton != null)
		{
			colonyCloudButton.onClick += delegate
			{
				ConvertAllToCloud();
			};
		}
		if (colonyLocalButton != null)
		{
			colonyLocalButton.onClick += delegate
			{
				ConvertAllToLocal();
			};
		}
		if (colonyInfoButton != null)
		{
			colonyInfoButton.onClick += delegate
			{
				ShowSaveInfo();
			};
		}
	}

	private bool IsInMenu()
	{
		return App.GetCurrentSceneName() == "frontend";
	}

	private bool CloudSavesVisible()
	{
		if (!SaveLoader.GetCloudSavesAvailable())
		{
			return false;
		}
		return IsInMenu();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		WorldGen.LoadSettings();
		SetCloudSaveInfoActive(CloudSavesVisible());
		RefreshColonyList();
		ShowColonyList();
		bool cloudSavesAvailable = SaveLoader.GetCloudSavesAvailable();
		cloudTutorialBouncer.gameObject.SetActive(cloudSavesAvailable);
		if (cloudSavesAvailable && !cloudTutorialBouncer.IsBouncing())
		{
			int @int = KPlayerPrefs.GetInt("LoadScreenCloudTutorialTimes", 0);
			if (@int < 5)
			{
				cloudTutorialBouncer.Bounce();
				KPlayerPrefs.SetInt("LoadScreenCloudTutorialTimes", @int + 1);
				KPlayerPrefs.GetInt("LoadScreenCloudTutorialTimes", 0);
			}
			else
			{
				cloudTutorialBouncer.gameObject.SetActive(value: false);
			}
		}
	}

	private Dictionary<string, List<SaveGameFileDetails>> GetColonies(List<string> files)
	{
		Dictionary<string, List<SaveGameFileDetails>> dictionary = new Dictionary<string, List<SaveGameFileDetails>>();
		if (files.Count <= 0)
		{
			return dictionary;
		}
		for (int i = 0; i < files.Count; i++)
		{
			if (IsFileValid(files[i]))
			{
				Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(files[i]);
				SaveGame.Header first = fileInfo.first;
				SaveGame.GameInfo second = fileInfo.second;
				System.DateTime lastWriteTime = File.GetLastWriteTime(files[i]);
				long size = 0L;
				try
				{
					size = new FileInfo(files[i]).Length;
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Failed to get size for file: " + files[i] + "\n" + ex.ToString());
				}
				SaveGameFileDetails item = default(SaveGameFileDetails);
				item.BaseName = second.baseName;
				item.FileName = files[i];
				item.FileDate = lastWriteTime;
				item.FileHeader = first;
				item.FileInfo = second;
				item.Size = size;
				item.UniqueID = SaveGame.GetSaveUniqueID(second);
				if (!dictionary.ContainsKey(item.UniqueID))
				{
					dictionary.Add(item.UniqueID, new List<SaveGameFileDetails>());
				}
				dictionary[item.UniqueID].Add(item);
			}
		}
		return dictionary;
	}

	private Dictionary<string, List<SaveGameFileDetails>> GetColonies()
	{
		List<string> allFiles = SaveLoader.GetAllFiles();
		return GetColonies(allFiles);
	}

	private Dictionary<string, List<SaveGameFileDetails>> GetLocalColonies()
	{
		List<string> allFiles = SaveLoader.GetAllFiles(SaveLoader.SaveType.local);
		return GetColonies(allFiles);
	}

	private Dictionary<string, List<SaveGameFileDetails>> GetCloudColonies()
	{
		List<string> allFiles = SaveLoader.GetAllFiles(SaveLoader.SaveType.cloud);
		return GetColonies(allFiles);
	}

	private bool IsFileValid(string filename)
	{
		bool result = false;
		try
		{
			result = SaveLoader.LoadHeader(filename, out var _).saveMajorVersion >= 7;
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Corrupted save file: " + filename + "\n" + ex.ToString());
			return result;
		}
	}

	private void CheckCloudLocalOverlap()
	{
		if (!SaveLoader.GetCloudSavesAvailable())
		{
			return;
		}
		string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
		if (cloudSavePrefix == null)
		{
			return;
		}
		foreach (KeyValuePair<string, List<SaveGameFileDetails>> colony in GetColonies())
		{
			bool flag = false;
			List<SaveGameFileDetails> list = new List<SaveGameFileDetails>();
			foreach (SaveGameFileDetails item in colony.Value)
			{
				if (SaveLoader.IsSaveCloud(item.FileName))
				{
					flag = true;
				}
				else
				{
					list.Add(item);
				}
			}
			if (!flag || list.Count == 0)
			{
				continue;
			}
			string baseName = list[0].BaseName;
			string path = System.IO.Path.Combine(SaveLoader.GetSavePrefix(), baseName);
			string text = System.IO.Path.Combine(cloudSavePrefix, baseName);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			Debug.Log("Saves / Found overlapped cloud/local saves for colony '" + baseName + "', moving to cloud...");
			foreach (SaveGameFileDetails item2 in list)
			{
				string fileName = item2.FileName;
				string source = System.IO.Path.ChangeExtension(fileName, "png");
				string path2 = text;
				if (SaveLoader.IsSaveAuto(fileName))
				{
					string text2 = System.IO.Path.Combine(path2, "auto_save");
					if (!Directory.Exists(text2))
					{
						Directory.CreateDirectory(text2);
					}
					path2 = text2;
				}
				string text3 = System.IO.Path.Combine(path2, System.IO.Path.GetFileName(fileName));
				if (FileMatch(fileName, text3, out var _))
				{
					Debug.Log("Saves / file match found for `" + fileName + "`...");
					MigrateFile(fileName, text3);
					string dest = System.IO.Path.ChangeExtension(text3, "png");
					MigrateFile(source, dest, ignoreMissing: true);
				}
				else
				{
					Debug.Log("Saves / no file match found for `" + fileName + "`... move as copy");
					string nextUsableSavePath = SaveLoader.GetNextUsableSavePath(text3);
					MigrateFile(fileName, nextUsableSavePath);
					string dest2 = System.IO.Path.ChangeExtension(nextUsableSavePath, "png");
					MigrateFile(source, dest2, ignoreMissing: true);
				}
			}
			RemoveEmptyFolder(path);
		}
	}

	private void DeleteFileAndEmptyFolder(string file)
	{
		if (File.Exists(file))
		{
			File.Delete(file);
		}
		RemoveEmptyFolder(System.IO.Path.GetDirectoryName(file));
	}

	private void RemoveEmptyFolder(string path)
	{
		if (Directory.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.Directory) && !Directory.EnumerateFileSystemEntries(path).Any())
		{
			try
			{
				Directory.Delete(path);
			}
			catch (Exception obj)
			{
				Debug.LogWarning("Failed to remove empty directory `" + path + "`...");
				Debug.LogWarning(obj);
			}
		}
	}

	private void RefreshColonyList()
	{
		if (colonyListPool != null)
		{
			colonyListPool.ClearAll();
		}
		CheckCloudLocalOverlap();
		Dictionary<string, List<SaveGameFileDetails>> colonies = GetColonies();
		if (colonies.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, List<SaveGameFileDetails>> item in colonies)
		{
			AddColonyToList(item.Value);
		}
	}

	private string GetFileHash(string path)
	{
		using MD5 mD = MD5.Create();
		using FileStream inputStream = File.OpenRead(path);
		return BitConverter.ToString(mD.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant();
	}

	private bool FileMatch(string file, string other_file, out Tuple<bool, bool> matches)
	{
		matches = new Tuple<bool, bool>(a: false, b: false);
		if (!File.Exists(file))
		{
			return false;
		}
		if (!File.Exists(other_file))
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		try
		{
			string fileHash = GetFileHash(file);
			string fileHash2 = GetFileHash(other_file);
			FileInfo fileInfo = new FileInfo(file);
			FileInfo fileInfo2 = new FileInfo(other_file);
			flag = fileInfo.Length == fileInfo2.Length;
			flag2 = fileHash == fileHash2;
		}
		catch (Exception obj)
		{
			Debug.LogWarning("FileMatch / file match failed for `" + file + "` vs `" + other_file + "`!");
			Debug.LogWarning(obj);
			return false;
		}
		matches.first = flag;
		matches.second = flag2;
		return flag && flag2;
	}

	private bool MigrateFile(string source, string dest, bool ignoreMissing = false)
	{
		Debug.Log("Migration / moving `" + source + "` to `" + dest + "` ...");
		if (dest == source)
		{
			Debug.Log("Migration / ignored `" + source + "` to `" + dest + "` ... same location");
			return true;
		}
		if (FileMatch(source, dest, out var _))
		{
			Debug.Log("Migration / dest and source are identical size + hash ... removing original");
			try
			{
				DeleteFileAndEmptyFolder(source);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Migration / removing original failed for `" + source + "`!");
				Debug.LogWarning(ex);
				throw ex;
			}
			return true;
		}
		try
		{
			Debug.Log("Migration / copying...");
			File.Copy(source, dest, overwrite: false);
		}
		catch (FileNotFoundException) when (ignoreMissing)
		{
			Debug.Log("Migration / File `" + source + "` wasn't found but we're ignoring that.");
			return true;
		}
		catch (Exception ex3)
		{
			Debug.LogWarning("Migration / copy failed for `" + source + "`! Leaving it alone");
			Debug.LogWarning(ex3);
			Debug.LogWarning("failed to convert colony: " + ex3.ToString());
			throw ex3;
		}
		Debug.Log("Migration / copy ok ...");
		if (!FileMatch(source, dest, out var matches2))
		{
			Debug.LogWarning("Migration / failed to match dest file for `" + source + "`!");
			Debug.LogWarning($"Migration / did hash match? {matches2.second} did size match? {matches2.first}");
			throw new Exception("Hash/Size didn't match for source and destination");
		}
		Debug.Log("Migration / hash validation ok ... removing original");
		try
		{
			DeleteFileAndEmptyFolder(source);
		}
		catch (Exception ex4)
		{
			Debug.LogWarning("Migration / removing original failed for `" + source + "`!");
			Debug.LogWarning(ex4);
			throw ex4;
		}
		Debug.Log("Migration / moved ok for `" + source + "`!");
		return true;
	}

	private bool MigrateSave(string dest_root, string file, bool is_auto_save, out string saveError)
	{
		saveError = null;
		Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(file);
		_ = fileInfo.first;
		string baseName = fileInfo.second.baseName;
		string fileName = System.IO.Path.GetFileName(file);
		string text = System.IO.Path.Combine(dest_root, baseName);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string path = text;
		if (is_auto_save)
		{
			string text2 = System.IO.Path.Combine(text, "auto_save");
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			path = text2;
		}
		string text3 = System.IO.Path.Combine(path, fileName);
		string source = System.IO.Path.ChangeExtension(file, "png");
		string dest = System.IO.Path.ChangeExtension(text3, "png");
		try
		{
			MigrateFile(file, text3);
			MigrateFile(source, dest, ignoreMissing: true);
		}
		catch (Exception ex)
		{
			saveError = ex.Message;
			return false;
		}
		return true;
	}

	private (int, int, ulong) GetSavesSizeAndCounts(List<SaveGameFileDetails> list)
	{
		ulong num = 0uL;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			SaveGameFileDetails saveGameFileDetails = list[i];
			num += (ulong)saveGameFileDetails.Size;
			if (saveGameFileDetails.FileInfo.isAutoSave)
			{
				num3++;
			}
			else
			{
				num2++;
			}
		}
		return (num2, num3, num);
	}

	private int CountValidSaves(string path, SearchOption searchType = SearchOption.AllDirectories)
	{
		int num = 0;
		List<string> saveFiles = SaveLoader.GetSaveFiles(path, searchType);
		for (int i = 0; i < saveFiles.Count; i++)
		{
			if (IsFileValid(saveFiles[i]))
			{
				num++;
			}
		}
		return num;
	}

	private (int, int) GetMigrationSaveCounts()
	{
		int item = CountValidSaves(SaveLoader.GetSavePrefixAndCreateFolder(), SearchOption.TopDirectoryOnly);
		int item2 = CountValidSaves(SaveLoader.GetAutoSavePrefix());
		return (item, item2);
	}

	private (int, int) MigrateSaves(out string errorColony, out string errorMessage)
	{
		errorColony = null;
		errorMessage = null;
		int num = 0;
		string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
		List<string> saveFiles = SaveLoader.GetSaveFiles(savePrefixAndCreateFolder, SearchOption.TopDirectoryOnly);
		for (int i = 0; i < saveFiles.Count; i++)
		{
			string text = saveFiles[i];
			if (IsFileValid(text))
			{
				if (MigrateSave(savePrefixAndCreateFolder, text, is_auto_save: false, out var saveError))
				{
					num++;
				}
				else if (errorColony == null)
				{
					errorColony = text;
					errorMessage = saveError;
				}
			}
		}
		int num2 = 0;
		List<string> saveFiles2 = SaveLoader.GetSaveFiles(SaveLoader.GetAutoSavePrefix());
		for (int j = 0; j < saveFiles2.Count; j++)
		{
			string text2 = saveFiles2[j];
			if (IsFileValid(text2))
			{
				if (MigrateSave(savePrefixAndCreateFolder, text2, is_auto_save: true, out var saveError2))
				{
					num2++;
				}
				else if (errorColony == null)
				{
					errorColony = text2;
					errorMessage = saveError2;
				}
			}
		}
		return (num, num2);
	}

	public void ShowMigrationIfNecessary(bool fromMainMenu)
	{
		var (saveCount, autoCount) = GetMigrationSaveCounts();
		if (saveCount == 0 && autoCount == 0)
		{
			if (fromMainMenu)
			{
				Deactivate();
			}
			return;
		}
		Activate();
		migrationPanelRefs.gameObject.SetActive(value: true);
		KButton migrateButton = migrationPanelRefs.GetReference<RectTransform>("MigrateSaves").GetComponent<KButton>();
		KButton continueButton = migrationPanelRefs.GetReference<RectTransform>("Continue").GetComponent<KButton>();
		KButton moreInfoButton = migrationPanelRefs.GetReference<RectTransform>("MoreInfo").GetComponent<KButton>();
		KButton component = migrationPanelRefs.GetReference<RectTransform>("OpenSaves").GetComponent<KButton>();
		LocText statsText = migrationPanelRefs.GetReference<RectTransform>("CountText").GetComponent<LocText>();
		LocText infoText = migrationPanelRefs.GetReference<RectTransform>("InfoText").GetComponent<LocText>();
		migrateButton.gameObject.SetActive(value: true);
		continueButton.gameObject.SetActive(value: false);
		moreInfoButton.gameObject.SetActive(value: false);
		statsText.text = string.Format(UI.FRONTEND.LOADSCREEN.MIGRATE_COUNT, saveCount, autoCount);
		component.ClearOnClick();
		component.onClick += delegate
		{
			Application.OpenURL(SaveLoader.GetSavePrefixAndCreateFolder());
		};
		migrateButton.ClearOnClick();
		migrateButton.onClick += delegate
		{
			migrateButton.gameObject.SetActive(value: false);
			string errorColony;
			string errorMessage;
			(int, int) tuple2 = MigrateSaves(out errorColony, out errorMessage);
			int item = tuple2.Item1;
			int item2 = tuple2.Item2;
			bool flag = errorColony == null;
			string format = (flag ? UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT.text : UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES.Replace("{ErrorColony}", errorColony).Replace("{ErrorMessage}", errorMessage));
			statsText.text = string.Format(format, item, saveCount, item2, autoCount);
			infoText.gameObject.SetActive(value: false);
			if (flag)
			{
				continueButton.gameObject.SetActive(value: true);
			}
			else
			{
				moreInfoButton.gameObject.SetActive(value: true);
			}
			MainMenu.Instance.RefreshResumeButton();
			RefreshColonyList();
		};
		continueButton.ClearOnClick();
		continueButton.onClick += delegate
		{
			migrationPanelRefs.gameObject.SetActive(value: false);
			cloudTutorialBouncer.Bounce();
		};
		moreInfoButton.ClearOnClick();
		moreInfoButton.onClick += delegate
		{
			Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, base.gameObject).SetHeader(UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_TITLE).AddPlainText(UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_PRE)
				.AddLineItem(UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM1, "")
				.AddLineItem(UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM2, "")
				.AddLineItem(UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM3, "")
				.AddPlainText(UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_POST)
				.AddOption(UI.FRONTEND.LOADSCREEN.MIGRATE_FAILURES_FORUM_BUTTON, delegate
				{
					Application.OpenURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/");
				})
				.AddOption(UI.CONFIRMDIALOG.OK, delegate(InfoDialogScreen d)
				{
					migrationPanelRefs.gameObject.SetActive(value: false);
					cloudTutorialBouncer.Bounce();
					d.Deactivate();
				}, rightSide: true)
				.Activate();
		};
	}

	private void SetCloudSaveInfoActive(bool active)
	{
		colonyCloudButton.gameObject.SetActive(active);
		colonyLocalButton.gameObject.SetActive(active);
	}

	private bool ConvertToLocalOrCloud(string fromRoot, string destRoot, string colonyName)
	{
		string text = System.IO.Path.Combine(fromRoot, colonyName);
		string text2 = System.IO.Path.Combine(destRoot, colonyName);
		Debug.Log("Convert / Colony '" + colonyName + "' from `" + text + "` => `" + text2 + "`");
		try
		{
			Directory.Move(text, text2);
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("failed to convert colony: " + ex.ToString());
			string message = UI.FRONTEND.LOADSCREEN.CONVERT_ERROR.Replace("{Colony}", colonyName).Replace("{Error}", ex.Message);
			ShowConvertError(message);
		}
		return false;
	}

	private bool ConvertColonyToCloud(string colonyName)
	{
		string savePrefix = SaveLoader.GetSavePrefix();
		string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
		if (cloudSavePrefix == null)
		{
			Debug.LogWarning("Failed to move colony to cloud, no cloud save prefix found (usually a userID is missing, not logged in?)");
			return false;
		}
		return ConvertToLocalOrCloud(savePrefix, cloudSavePrefix, colonyName);
	}

	private bool ConvertColonyToLocal(string colonyName)
	{
		string savePrefix = SaveLoader.GetSavePrefix();
		string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
		if (cloudSavePrefix == null)
		{
			Debug.LogWarning("Failed to move colony from cloud, no cloud save prefix found (usually a userID is missing, not logged in?)");
			return false;
		}
		return ConvertToLocalOrCloud(cloudSavePrefix, savePrefix, colonyName);
	}

	private void DoConvertAllToLocal()
	{
		Dictionary<string, List<SaveGameFileDetails>> cloudColonies = GetCloudColonies();
		if (cloudColonies.Count == 0)
		{
			return;
		}
		bool flag = true;
		foreach (KeyValuePair<string, List<SaveGameFileDetails>> item in cloudColonies)
		{
			flag &= ConvertColonyToLocal(item.Value[0].BaseName);
		}
		if (flag)
		{
			string replacement = UI.PLATFORMS.STEAM;
			ShowSimpleDialog(UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL, UI.FRONTEND.LOADSCREEN.CONVERT_ALL_TO_LOCAL_SUCCESS.Replace("{Client}", replacement));
		}
		RefreshColonyList();
		MainMenu.Instance.RefreshResumeButton();
		SaveLoader.SetCloudSavesDefault(value: false);
	}

	private void DoConvertAllToCloud()
	{
		Dictionary<string, List<SaveGameFileDetails>> localColonies = GetLocalColonies();
		if (localColonies.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, List<SaveGameFileDetails>> item in localColonies)
		{
			string baseName = item.Value[0].BaseName;
			if (!list.Contains(baseName))
			{
				list.Add(baseName);
			}
		}
		bool flag = true;
		foreach (string item2 in list)
		{
			flag &= ConvertColonyToCloud(item2);
		}
		if (flag)
		{
			string replacement = UI.PLATFORMS.STEAM;
			ShowSimpleDialog(UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD, UI.FRONTEND.LOADSCREEN.CONVERT_ALL_TO_CLOUD_SUCCESS.Replace("{Client}", replacement));
		}
		RefreshColonyList();
		MainMenu.Instance.RefreshResumeButton();
		SaveLoader.SetCloudSavesDefault(value: true);
	}

	private void ConvertAllToCloud()
	{
		string message = $"{UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD_DETAILS}\n{UI.FRONTEND.LOADSCREEN.CONVERT_ALL_WARNING}\n";
		KPlayerPrefs.SetInt("LoadScreenCloudTutorialTimes", 5);
		ConfirmCloudSaveMigrations(message, UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD, UI.FRONTEND.LOADSCREEN.CONVERT_ALL_COLONIES, UI.FRONTEND.LOADSCREEN.OPEN_SAVE_FOLDER, delegate
		{
			DoConvertAllToCloud();
		}, delegate
		{
			Application.OpenURL(SaveLoader.GetSavePrefix());
		}, localToCloudSprite);
	}

	private void ConvertAllToLocal()
	{
		string message = $"{UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL_DETAILS}\n{UI.FRONTEND.LOADSCREEN.CONVERT_ALL_WARNING}\n";
		KPlayerPrefs.SetInt("LoadScreenCloudTutorialTimes", 5);
		ConfirmCloudSaveMigrations(message, UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL, UI.FRONTEND.LOADSCREEN.CONVERT_ALL_COLONIES, UI.FRONTEND.LOADSCREEN.OPEN_SAVE_FOLDER, delegate
		{
			DoConvertAllToLocal();
		}, delegate
		{
			Application.OpenURL(SaveLoader.GetCloudSavePrefix());
		}, cloudToLocalSprite);
	}

	private void ShowSaveInfo()
	{
		if (!(infoScreen == null))
		{
			return;
		}
		infoScreen = Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, base.gameObject).SetHeader(UI.FRONTEND.LOADSCREEN.SAVE_INFO_DIALOG_TITLE).AddSprite(infoSprite)
			.AddPlainText(UI.FRONTEND.LOADSCREEN.SAVE_INFO_DIALOG_TEXT)
			.AddOption(UI.FRONTEND.LOADSCREEN.OPEN_SAVE_FOLDER, delegate
			{
				Application.OpenURL(SaveLoader.GetSavePrefix());
			}, rightSide: true)
			.AddDefaultCancel();
		string cloudRoot = SaveLoader.GetCloudSavePrefix();
		if (cloudRoot != null && CloudSavesVisible())
		{
			infoScreen.AddOption(UI.FRONTEND.LOADSCREEN.OPEN_CLOUDSAVE_FOLDER, delegate
			{
				Application.OpenURL(cloudRoot);
			}, rightSide: true);
		}
		infoScreen.gameObject.SetActive(value: true);
	}

	protected override void OnDeactivate()
	{
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Unpause(playSound: false);
		}
		selectedSave = null;
		base.OnDeactivate();
	}

	private void ShowColonyList()
	{
		colonyListRoot.SetActive(value: true);
		colonyViewRoot.SetActive(value: false);
		currentColony = null;
		selectedSave = null;
	}

	private bool CheckSave(SaveGameFileDetails save, LocText display)
	{
		if (IsSaveFileFromSpacedOut(save.FileHeader, save.FileInfo))
		{
			if (display != null)
			{
				display.text = string.Format(UI.FRONTEND.LOADSCREEN.SAVE_FROM_SPACED_OUT, save.FileInfo.saveMinorVersion, 17);
			}
			return false;
		}
		if (IsSaveFileFromUnsupportedFutureBuild(save.FileHeader, save.FileInfo))
		{
			if (display != null)
			{
				display.text = string.Format(UI.FRONTEND.LOADSCREEN.SAVE_TOO_NEW, save.FileName, save.FileHeader.buildVersion, save.FileInfo.saveMinorVersion, 447596u, 17);
			}
			return false;
		}
		if (save.FileInfo.saveMajorVersion < 7)
		{
			if (display != null)
			{
				display.text = string.Format(UI.FRONTEND.LOADSCREEN.UNSUPPORTED_SAVE_VERSION, save.FileName, save.FileInfo.saveMajorVersion, save.FileInfo.saveMinorVersion, 7, 17);
			}
			return false;
		}
		return true;
	}

	private void ShowColonySave(SaveGameFileDetails save)
	{
		HierarchyReferences component = colonyViewRoot.GetComponent<HierarchyReferences>();
		component.GetReference<RectTransform>("Title").GetComponent<LocText>().text = save.BaseName;
		component.GetReference<RectTransform>("Date").GetComponent<LocText>().text = string.Format("{0:H:mm:ss} - " + Localization.GetFileDateFormat(0), save.FileDate);
		LocText reference = component.GetReference<LocText>("InfoWorld");
		string text = save.FileInfo.worldID;
		if (text == null)
		{
			text = "worlds/SandstoneDefault";
		}
		ProcGen.World worldData = SettingsCache.worlds.GetWorldData(text);
		reference.text = string.Format(arg1: (worldData != null) ? ((string)Strings.Get(worldData.name)) : " - ", format: UI.FRONTEND.LOADSCREEN.COLONY_INFO_FMT, arg0: UI.FRONTEND.LOADSCREEN.WORLD_NAME);
		component.GetReference<LocText>("InfoCycles").text = string.Format(UI.FRONTEND.LOADSCREEN.COLONY_INFO_FMT, UI.FRONTEND.LOADSCREEN.CYCLES_SURVIVED, save.FileInfo.numberOfCycles);
		component.GetReference<LocText>("InfoDupes").text = string.Format(UI.FRONTEND.LOADSCREEN.COLONY_INFO_FMT, UI.FRONTEND.LOADSCREEN.DUPLICANTS_ALIVE, save.FileInfo.numberOfDuplicants);
		component.GetReference<RectTransform>("FileSize").GetComponent<LocText>().text = string.Format(arg0: GameUtil.GetFormattedBytes((ulong)save.Size), format: UI.FRONTEND.LOADSCREEN.COLONY_FILE_SIZE);
		component.GetReference<RectTransform>("Filename").GetComponent<LocText>().text = string.Format(UI.FRONTEND.LOADSCREEN.COLONY_FILE_NAME, System.IO.Path.GetFileName(save.FileName));
		LocText component2 = component.GetReference<RectTransform>("AutoInfo").GetComponent<LocText>();
		component2.gameObject.SetActive(!CheckSave(save, component2));
		SetPreview(preview: component.GetReference<RectTransform>("Preview").GetComponent<Image>(), filename: save.FileName, basename: save.BaseName);
		KButton component4 = component.GetReference<RectTransform>("DeleteButton").GetComponent<KButton>();
		component4.ClearOnClick();
		component4.onClick += delegate
		{
			Delete(delegate
			{
				int num = currentColony.IndexOf(save);
				currentColony.Remove(save);
				ShowColony(currentColony, num - 1);
			});
		};
	}

	private void ShowColony(List<SaveGameFileDetails> saves, int selectIndex = -1)
	{
		if (saves.Count <= 0)
		{
			RefreshColonyList();
			ShowColonyList();
			return;
		}
		currentColony = saves;
		colonyListRoot.SetActive(value: false);
		colonyViewRoot.SetActive(value: true);
		string baseName = saves[0].BaseName;
		HierarchyReferences component = colonyViewRoot.GetComponent<HierarchyReferences>();
		KButton component2 = component.GetReference<RectTransform>("Back").GetComponent<KButton>();
		component2.ClearOnClick();
		component2.onClick += delegate
		{
			ShowColonyList();
		};
		component.GetReference<RectTransform>("ColonyTitle").GetComponent<LocText>().text = string.Format(UI.FRONTEND.LOADSCREEN.COLONY_TITLE, baseName);
		GameObject gameObject = component.GetReference<RectTransform>("Content").gameObject;
		RectTransform reference = component.GetReference<RectTransform>("SaveTemplate");
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			if (gameObject2 != null && gameObject2.name.Contains("Clone"))
			{
				UnityEngine.Object.Destroy(gameObject2);
			}
		}
		if (selectIndex < 0)
		{
			selectIndex = 0;
		}
		if (selectIndex > saves.Count - 1)
		{
			selectIndex = saves.Count - 1;
		}
		for (int j = 0; j < saves.Count; j++)
		{
			SaveGameFileDetails save = saves[j];
			RectTransform rectTransform = UnityEngine.Object.Instantiate(reference, gameObject.transform);
			HierarchyReferences component3 = rectTransform.GetComponent<HierarchyReferences>();
			rectTransform.gameObject.SetActive(value: true);
			component3.GetReference<RectTransform>("AutoLabel").gameObject.SetActive(save.FileInfo.isAutoSave);
			component3.GetReference<RectTransform>("SaveText").GetComponent<LocText>().text = System.IO.Path.GetFileNameWithoutExtension(save.FileName);
			component3.GetReference<RectTransform>("DateText").GetComponent<LocText>().text = string.Format("{0:H:mm:ss} - " + Localization.GetFileDateFormat(0), save.FileDate);
			component3.GetReference<RectTransform>("NewestLabel").gameObject.SetActive(j == 0);
			bool num = CheckSave(save, null);
			KButton button = rectTransform.GetComponent<KButton>();
			button.ClearOnClick();
			button.onClick += delegate
			{
				UpdateSelected(button, save.FileName);
				ShowColonySave(save);
			};
			if (num)
			{
				button.onDoubleClick += delegate
				{
					UpdateSelected(button, save.FileName);
					Load();
				};
			}
			KButton component4 = component3.GetReference<RectTransform>("LoadButton").GetComponent<KButton>();
			component4.ClearOnClick();
			if (!num)
			{
				component4.isInteractable = false;
				component4.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Disabled);
			}
			else
			{
				component4.onClick += delegate
				{
					UpdateSelected(button, save.FileName);
					Load();
				};
			}
			if (j == selectIndex)
			{
				UpdateSelected(button, save.FileName);
				ShowColonySave(save);
			}
		}
	}

	private void AddColonyToList(List<SaveGameFileDetails> saves)
	{
		if (saves.Count == 0)
		{
			return;
		}
		HierarchyReferences freeElement = colonyListPool.GetFreeElement(saveButtonRoot, forceActive: true);
		saves.Sort((SaveGameFileDetails x, SaveGameFileDetails y) => y.FileDate.CompareTo(x.FileDate));
		SaveGameFileDetails firstSave = saves[0];
		bool flag = IsSaveFileFromSpacedOut(firstSave.FileHeader, firstSave.FileInfo);
		string colonyName = firstSave.BaseName;
		(int, int, ulong) savesSizeAndCounts = GetSavesSizeAndCounts(saves);
		int item = savesSizeAndCounts.Item1;
		int item2 = savesSizeAndCounts.Item2;
		string formattedBytes = GameUtil.GetFormattedBytes(savesSizeAndCounts.Item3);
		freeElement.GetReference<RectTransform>("HeaderTitle").GetComponent<LocText>().text = colonyName;
		freeElement.GetReference<RectTransform>("HeaderDate").GetComponent<LocText>().text = string.Format("{0:H:mm:ss} - " + Localization.GetFileDateFormat(0), firstSave.FileDate);
		freeElement.GetReference<RectTransform>("SaveTitle").GetComponent<LocText>().text = string.Format(UI.FRONTEND.LOADSCREEN.SAVE_INFO, item, item2, formattedBytes);
		Image component = freeElement.GetReference<RectTransform>("Preview").GetComponent<Image>();
		SetPreview(firstSave.FileName, colonyName, component, fallbackToTimelapse: true);
		KImage reference = freeElement.GetReference<KImage>("DlcIcon");
		reference.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.LOADSCREEN.SAVE_FROM_SPACED_OUT_TOOLTIP);
		RectTransform reference2 = freeElement.GetReference<RectTransform>("LocationIcons");
		bool flag2 = CloudSavesVisible();
		reference2.gameObject.SetActive(flag2);
		if (flag2)
		{
			LocText locationText = freeElement.GetReference<RectTransform>("LocationText").GetComponent<LocText>();
			bool isLocal = SaveLoader.IsSaveLocal(firstSave.FileName);
			locationText.text = (isLocal ? UI.FRONTEND.LOADSCREEN.LOCAL_SAVE : UI.FRONTEND.LOADSCREEN.CLOUD_SAVE);
			KButton cloudButton = freeElement.GetReference<RectTransform>("CloudButton").GetComponent<KButton>();
			KButton localButton = freeElement.GetReference<RectTransform>("LocalButton").GetComponent<KButton>();
			cloudButton.gameObject.SetActive(!isLocal);
			cloudButton.ClearOnClick();
			cloudButton.onClick += delegate
			{
				string message2 = $"{UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL_DETAILS}\n";
				ConfirmCloudSaveMigrations(message2, UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL, UI.FRONTEND.LOADSCREEN.CONVERT_COLONY, null, delegate
				{
					cloudButton.gameObject.SetActive(value: false);
					isLocal = true;
					locationText.text = (isLocal ? UI.FRONTEND.LOADSCREEN.LOCAL_SAVE : UI.FRONTEND.LOADSCREEN.CLOUD_SAVE);
					ConvertColonyToLocal(colonyName);
					RefreshColonyList();
					MainMenu.Instance.RefreshResumeButton();
				}, null, cloudToLocalSprite);
			};
			localButton.gameObject.SetActive(isLocal);
			localButton.ClearOnClick();
			localButton.onClick += delegate
			{
				string message = $"{UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD_DETAILS}\n";
				ConfirmCloudSaveMigrations(message, UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD, UI.FRONTEND.LOADSCREEN.CONVERT_COLONY, null, delegate
				{
					localButton.gameObject.SetActive(value: false);
					isLocal = false;
					locationText.text = (isLocal ? UI.FRONTEND.LOADSCREEN.LOCAL_SAVE : UI.FRONTEND.LOADSCREEN.CLOUD_SAVE);
					ConvertColonyToCloud(colonyName);
					RefreshColonyList();
					MainMenu.Instance.RefreshResumeButton();
				}, null, localToCloudSprite);
			};
		}
		KButton component2 = freeElement.GetReference<RectTransform>("Button").GetComponent<KButton>();
		component2.ClearOnClick();
		component2.isInteractable = !flag;
		reference.gameObject.SetActive(flag);
		component2.onClick += delegate
		{
			ShowColony(saves);
		};
		if (CheckSave(firstSave, null))
		{
			component2.onDoubleClick += delegate
			{
				UpdateSelected(null, firstSave.FileName);
				Load();
			};
		}
		freeElement.transform.SetAsLastSibling();
	}

	private void SetPreview(string filename, string basename, Image preview, bool fallbackToTimelapse = false)
	{
		preview.color = Color.black;
		preview.gameObject.SetActive(value: false);
		try
		{
			Sprite sprite = RetireColonyUtility.LoadColonyPreview(filename, basename, fallbackToTimelapse);
			if (!(sprite == null))
			{
				Rect rect = preview.rectTransform.parent.rectTransform().rect;
				preview.sprite = sprite;
				preview.color = (sprite ? Color.white : Color.black);
				float num = sprite.bounds.size.x / sprite.bounds.size.y;
				if ((double)num >= 1.77777777777778)
				{
					preview.rectTransform.sizeDelta = new Vector2(rect.height * num, rect.height);
				}
				else
				{
					preview.rectTransform.sizeDelta = new Vector2(rect.width, rect.width / num);
				}
				preview.gameObject.SetActive(value: true);
			}
		}
		catch (Exception obj)
		{
			Debug.Log(obj);
		}
	}

	public static void ForceStopGame()
	{
		ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
		Game.Instance.SetIsLoading();
		Grid.CellCount = 0;
		Sim.Shutdown();
	}

	private static bool IsSaveFileFromUnsupportedFutureBuild(SaveGame.Header header, SaveGame.GameInfo gameInfo)
	{
		if (gameInfo.saveMajorVersion > 7 || (gameInfo.saveMajorVersion == 7 && gameInfo.saveMinorVersion > 17))
		{
			return true;
		}
		return header.buildVersion > 447596;
	}

	private static bool IsSaveFileFromSpacedOut(SaveGame.Header header, SaveGame.GameInfo gameInfo)
	{
		if (gameInfo.saveMajorVersion == 7 && gameInfo.saveMinorVersion > 17)
		{
			return true;
		}
		return false;
	}

	private void UpdateSelected(KButton button, string filename)
	{
		if (selectedSave != null && selectedSave.button != null)
		{
			selectedSave.button.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
		if (selectedSave == null)
		{
			selectedSave = new SelectedSave();
		}
		selectedSave.button = button;
		selectedSave.filename = filename;
		if (selectedSave.button != null)
		{
			selectedSave.button.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		}
	}

	private void Load()
	{
		LoadingOverlay.Load(DoLoad);
	}

	private void DoLoad()
	{
		if (selectedSave != null)
		{
			DoLoad(selectedSave.filename);
			Deactivate();
		}
	}

	private static void DoLoad(string filename)
	{
		ReportErrorDialog.MOST_RECENT_SAVEFILE = filename;
		SaveGame.Header header;
		SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
		string arg = null;
		string arg2 = null;
		if (header.buildVersion > 447596)
		{
			arg = header.buildVersion.ToString();
			arg2 = 447596u.ToString();
		}
		else if (gameInfo.saveMajorVersion < 7)
		{
			arg = $"v{gameInfo.saveMajorVersion}.{gameInfo.saveMinorVersion}";
			arg2 = $"v{7}.{17}";
		}
		if (1 == 0)
		{
			GameObject parent = ((FrontEndManager.Instance == null) ? GameScreenManager.Instance.ssOverlayCanvas : FrontEndManager.Instance.gameObject);
			Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent, force_active: true).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(string.Format(UI.CRASHSCREEN.LOADFAILED, "Version Mismatch", arg, arg2), null, null);
			return;
		}
		if (Game.Instance != null)
		{
			ForceStopGame();
		}
		SaveLoader.SetActiveSaveFilePath(filename);
		Time.timeScale = 0f;
		App.LoadScene("backend");
	}

	private void MoreInfo()
	{
		Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2776550");
	}

	private void Delete(System.Action onDelete)
	{
		if (selectedSave == null || string.IsNullOrEmpty(selectedSave.filename))
		{
			Debug.LogError("The path provided is not valid and cannot be deleted.");
			return;
		}
		ConfirmDoAction(string.Format(UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, System.IO.Path.GetFileName(selectedSave.filename)), delegate
		{
			try
			{
				DeleteFileAndEmptyFolder(selectedSave.filename);
				string file = System.IO.Path.ChangeExtension(selectedSave.filename, "png");
				DeleteFileAndEmptyFolder(file);
				if (onDelete != null)
				{
					onDelete();
				}
			}
			catch (SystemException ex)
			{
				Debug.LogError(ex.ToString());
			}
		});
	}

	private void ShowSimpleDialog(string title, string message)
	{
		Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, base.gameObject).SetHeader(title).AddPlainText(message)
			.AddDefaultOK()
			.Activate();
	}

	private void ConfirmCloudSaveMigrations(string message, string title, string confirmText, string backupText, System.Action commitAction, System.Action backupAction, Sprite sprite)
	{
		Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, base.gameObject).SetHeader(title).AddSprite(sprite)
			.AddPlainText(message)
			.AddDefaultCancel()
			.AddOption(confirmText, delegate(InfoDialogScreen d)
			{
				d.Deactivate();
				commitAction();
			}, rightSide: true)
			.Activate();
	}

	private void ShowConvertError(string message)
	{
		if (errorInfoScreen == null)
		{
			errorInfoScreen = Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, base.gameObject).SetHeader(UI.FRONTEND.LOADSCREEN.CONVERT_ERROR_TITLE).AddSprite(errorSprite)
				.AddPlainText(message)
				.AddOption(UI.FRONTEND.LOADSCREEN.MIGRATE_FAILURES_FORUM_BUTTON, delegate
				{
					Application.OpenURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/");
				})
				.AddDefaultOK();
			errorInfoScreen.Activate();
		}
	}

	private void ConfirmDoAction(string message, System.Action action)
	{
		if (confirmScreen == null)
		{
			confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject);
			confirmScreen.PopupConfirmDialog(message, action, delegate
			{
			});
			confirmScreen.gameObject.SetActive(value: true);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (currentColony != null && e.TryConsume(Action.Escape))
		{
			ShowColonyList();
		}
		base.OnKeyDown(e);
	}
}
