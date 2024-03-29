using System;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using Klei;
using Steamworks;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : KScreen
{
	private struct ButtonInfo
	{
		public LocString text;

		public System.Action action;

		public int fontSize;

		public ColorStyleSetting style;

		public ButtonInfo(LocString text, System.Action action, int font_size, ColorStyleSetting style)
		{
			this.text = text;
			this.action = action;
			fontSize = font_size;
			this.style = style;
		}
	}

	private struct SaveFileEntry
	{
		public System.DateTime timeStamp;

		public SaveGame.Header header;

		public SaveGame.GameInfo headerData;
	}

	private static MainMenu _instance;

	public KButton Button_ResumeGame;

	private KButton Button_NewGame;

	public GameObject topLeftAlphaMessage;

	private MotdServerClient m_motdServerClient;

	private GameObject GameSettingsScreen;

	private bool m_screenshotMode;

	[SerializeField]
	private CanvasGroup uiCanvas;

	[SerializeField]
	private KButton buttonPrefab;

	[SerializeField]
	private GameObject buttonParent;

	[SerializeField]
	private ColorStyleSetting topButtonStyle;

	[SerializeField]
	private ColorStyleSetting normalButtonStyle;

	[SerializeField]
	private string menuMusicEventName;

	[SerializeField]
	private string ambientLoopEventName;

	private EventInstance ambientLoop;

	[SerializeField]
	private GameObject MOTDContainer;

	[SerializeField]
	private GameObject buttonContainer;

	[SerializeField]
	private LocText motdImageHeader;

	[SerializeField]
	private KButton motdImageButton;

	[SerializeField]
	private Image motdImage;

	[SerializeField]
	private LocText motdNewsHeader;

	[SerializeField]
	private LocText motdNewsBody;

	[SerializeField]
	private PatchNotesScreen patchNotesScreenPrefab;

	[SerializeField]
	private NextUpdateTimer nextUpdateTimer;

	[SerializeField]
	private DLCToggle expansion1Toggle;

	[SerializeField]
	private BuildWatermark buildWatermark;

	[SerializeField]
	public string IntroShortName;

	private static bool HasAutoresumedOnce = false;

	private bool refreshResumeButton = true;

	private int m_cheatInputCounter;

	public const string AutoResumeSaveFileKey = "AutoResumeSaveFile";

	public const string PLAY_SHORT_ON_LAUNCH = "PlayShortOnLaunch";

	private static int LANGUAGE_CONFIRMATION_VERSION = 2;

	private Dictionary<string, SaveFileEntry> saveFileEntries = new Dictionary<string, SaveFileEntry>();

	public static MainMenu Instance => _instance;

	private KButton MakeButton(ButtonInfo info)
	{
		KButton kButton = Util.KInstantiateUI<KButton>(buttonPrefab.gameObject, buttonParent, force_active: true);
		kButton.onClick += info.action;
		KImage component = kButton.GetComponent<KImage>();
		component.colorStyleSetting = info.style;
		component.ApplyColorStyleSetting();
		LocText componentInChildren = kButton.GetComponentInChildren<LocText>();
		componentInChildren.text = info.text;
		componentInChildren.fontSize = info.fontSize;
		return kButton;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		_instance = this;
		Button_NewGame = MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.NEWGAME, NewGame, 22, topButtonStyle));
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.LOADGAME, LoadGame, 22, normalButtonStyle));
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.RETIREDCOLONIES, delegate
		{
			ActivateRetiredColoniesScreen(base.transform.gameObject);
		}, 14, normalButtonStyle));
		if (DistributionPlatform.Initialized)
		{
			MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.TRANSLATIONS, Translations, 14, normalButtonStyle));
			MakeButton(new ButtonInfo(UI.FRONTEND.MODS.TITLE, Mods, 14, normalButtonStyle));
		}
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.OPTIONS, Options, 14, normalButtonStyle));
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.QUITTODESKTOP, QuitGame, 14, normalButtonStyle));
		RefreshResumeButton();
		Button_ResumeGame.onClick += ResumeGame;
		SpawnVideoScreen();
		StartFEAudio();
		CheckPlayerPrefsCorruption();
		if (PatchNotesScreen.ShouldShowScreen())
		{
			Util.KInstantiateUI(patchNotesScreenPrefab.gameObject, FrontEndManager.Instance.gameObject, force_active: true);
		}
		CheckDoubleBoundKeys();
		topLeftAlphaMessage.gameObject.SetActive(value: false);
		MOTDContainer.SetActive(value: false);
		buttonContainer.SetActive(value: false);
		bool active = DistributionPlatform.Inst.IsDLCPurchased("EXPANSION1_ID");
		nextUpdateTimer.gameObject.SetActive(active);
		expansion1Toggle.gameObject.SetActive(active);
		m_motdServerClient = new MotdServerClient();
		m_motdServerClient.GetMotd(delegate(MotdServerClient.MotdResponse response, string error)
		{
			if (error == null)
			{
				if (DlcManager.IsExpansion1Active())
				{
					nextUpdateTimer.UpdateReleaseTimes(response.expansion1_update_data.last_update_time, response.expansion1_update_data.next_update_time, response.expansion1_update_data.update_text_override);
				}
				else
				{
					nextUpdateTimer.UpdateReleaseTimes(response.vanilla_update_data.last_update_time, response.vanilla_update_data.next_update_time, response.vanilla_update_data.update_text_override);
				}
				topLeftAlphaMessage.gameObject.SetActive(value: true);
				MOTDContainer.SetActive(value: true);
				buttonContainer.SetActive(value: true);
				motdImageHeader.text = response.image_header_text;
				motdNewsHeader.text = response.news_header_text;
				motdNewsBody.text = response.news_body_text;
				PatchNotesScreen.UpdatePatchNotes(response.patch_notes_summary, response.patch_notes_link_url);
				if (response.image_texture != null)
				{
					motdImage.sprite = Sprite.Create(response.image_texture, new Rect(0f, 0f, response.image_texture.width, response.image_texture.height), Vector2.zero);
				}
				else
				{
					Debug.LogWarning("GetMotd failed to return an image texture");
				}
				if (motdImage.sprite != null && motdImage.sprite.rect.height != 0f)
				{
					AspectRatioFitter component = motdImage.gameObject.GetComponent<AspectRatioFitter>();
					if (component != null)
					{
						float num2 = (component.aspectRatio = motdImage.sprite.rect.width / motdImage.sprite.rect.height);
					}
					else
					{
						Debug.LogWarning("Missing AspectRatioFitter on MainMenu motd image.");
					}
				}
				else
				{
					Debug.LogWarning("Cannot resize motd image, missing sprite");
				}
				motdImageButton.ClearOnClick();
				motdImageButton.onClick += delegate
				{
					App.OpenWebURL(response.image_link_url);
				};
			}
			else
			{
				Debug.LogWarning("Motd Request error: " + error);
			}
		});
		if (DistributionPlatform.Initialized && DistributionPlatform.Inst.IsPreviousVersionBranch)
		{
			UnityEngine.Object.Instantiate(ScreenPrefabs.Instance.OldVersionWarningScreen, uiCanvas.transform);
		}
		activateOnSpawn = true;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			RefreshResumeButton();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		base.OnKeyDown(e);
		if (e.Consumed)
		{
			return;
		}
		if (e.TryConsume(Action.DebugToggleUI))
		{
			m_screenshotMode = !m_screenshotMode;
			uiCanvas.alpha = (m_screenshotMode ? 0f : 1f);
		}
		KKeyCode key_code = m_cheatInputCounter switch
		{
			0 => KKeyCode.K, 
			1 => KKeyCode.L, 
			2 => KKeyCode.E, 
			3 => KKeyCode.I, 
			4 => KKeyCode.P, 
			5 => KKeyCode.L, 
			6 => KKeyCode.A, 
			_ => KKeyCode.Y, 
		};
		if (e.Controller.GetKeyDown(key_code))
		{
			e.Consumed = true;
			m_cheatInputCounter++;
			if (m_cheatInputCounter >= 8)
			{
				Debug.Log("Cheat Detected - enabling Debug Mode");
				DebugHandler.SetDebugEnabled(debugEnabled: true);
				buildWatermark.RefreshText();
				m_cheatInputCounter = 0;
			}
		}
		else
		{
			m_cheatInputCounter = 0;
		}
	}

	private void PlayMouseOverSound()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
	}

	private void PlayMouseClickSound()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));
	}

	protected override void OnSpawn()
	{
		Debug.Log("-- MAIN MENU -- ");
		base.OnSpawn();
		m_cheatInputCounter = 0;
		Canvas.ForceUpdateCanvases();
		ShowLanguageConfirmation();
		InitLoadScreen();
		LoadScreen.Instance.ShowMigrationIfNecessary(fromMainMenu: true);
		string savePrefix = SaveLoader.GetSavePrefix();
		try
		{
			string path = Path.Combine(savePrefix, "__SPCCHK");
			using (FileStream fileStream = File.OpenWrite(path))
			{
				byte[] array = new byte[1024];
				for (int i = 0; i < 15360; i++)
				{
					fileStream.Write(array, 0, array.Length);
				}
			}
			File.Delete(path);
		}
		catch (Exception ex)
		{
			string format = ((!(ex is IOException)) ? string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, savePrefix) : string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, savePrefix));
			string text = string.Format(format, savePrefix);
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, force_active: true).PopupConfirmDialog(text, null, null);
		}
		Global.Instance.modManager.Report(base.gameObject);
		if ((GenericGameSettings.instance.autoResumeGame && !HasAutoresumedOnce && !KCrashReporter.hasCrash) || !string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) || KPlayerPrefs.HasKey("AutoResumeSaveFile"))
		{
			HasAutoresumedOnce = true;
			ResumeGame();
		}
		if (GenericGameSettings.instance.devAutoWorldGen && !KCrashReporter.hasCrash)
		{
			GenericGameSettings.instance.devAutoWorldGen = false;
			GenericGameSettings.instance.devAutoWorldGenActive = true;
			GenericGameSettings.instance.SaveSettings();
			Util.KInstantiateUI(ScreenPrefabs.Instance.WorldGenScreen.gameObject, base.gameObject, force_active: true);
		}
	}

	private void UnregisterMotdRequest()
	{
		if (m_motdServerClient != null)
		{
			m_motdServerClient.UnregisterCallback();
			m_motdServerClient = null;
		}
	}

	protected override void OnActivate()
	{
		if (!ambientLoopEventName.IsNullOrWhiteSpace())
		{
			ambientLoop = KFMOD.CreateInstance(GlobalAssets.GetSound(ambientLoopEventName));
			if (ambientLoop.isValid())
			{
				ambientLoop.start();
			}
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		UnregisterMotdRequest();
	}

	public override void ScreenUpdate(bool topLevel)
	{
		refreshResumeButton = topLevel;
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		StopAmbience();
		UnregisterMotdRequest();
	}

	private void ShowLanguageConfirmation()
	{
		if (SteamManager.Initialized && !(SteamUtils.GetSteamUILanguage() != "schinese") && KPlayerPrefs.GetInt("LanguageConfirmationVersion") < LANGUAGE_CONFIRMATION_VERSION)
		{
			KPlayerPrefs.SetInt("LanguageConfirmationVersion", LANGUAGE_CONFIRMATION_VERSION);
			Translations();
		}
	}

	private void ResumeGame()
	{
		string text;
		if (!KPlayerPrefs.HasKey("AutoResumeSaveFile"))
		{
			text = (string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) ? SaveLoader.GetLatestSaveForCurrentDLC() : GenericGameSettings.instance.performanceCapture.saveGame);
		}
		else
		{
			text = KPlayerPrefs.GetString("AutoResumeSaveFile");
			KPlayerPrefs.DeleteKey("AutoResumeSaveFile");
		}
		if (!string.IsNullOrEmpty(text))
		{
			KCrashReporter.MOST_RECENT_SAVEFILE = text;
			SaveLoader.SetActiveSaveFilePath(text);
			LoadingOverlay.Load(delegate
			{
				App.LoadScene("backend");
			});
		}
	}

	private void NewGame()
	{
		GetComponent<NewGameFlow>().BeginFlow();
	}

	private void InitLoadScreen()
	{
		if (LoadScreen.Instance == null)
		{
			Util.KInstantiateUI(ScreenPrefabs.Instance.LoadScreen.gameObject, base.gameObject, force_active: true).GetComponent<LoadScreen>();
		}
	}

	private void LoadGame()
	{
		InitLoadScreen();
		LoadScreen.Instance.Activate();
	}

	public static void ActivateRetiredColoniesScreen(GameObject parent, string colonyID = "")
	{
		if (RetiredColonyInfoScreen.Instance == null)
		{
			Util.KInstantiateUI(ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject, parent, force_active: true);
		}
		RetiredColonyInfoScreen.Instance.Show();
		if (!string.IsNullOrEmpty(colonyID))
		{
			if (SaveGame.Instance != null)
			{
				RetireColonyUtility.SaveColonySummaryData();
			}
			RetiredColonyInfoScreen.Instance.LoadColony(RetiredColonyInfoScreen.Instance.GetColonyDataByBaseName(colonyID));
		}
	}

	public static void ActivateRetiredColoniesScreenFromData(GameObject parent, RetiredColonyData data)
	{
		if (RetiredColonyInfoScreen.Instance == null)
		{
			Util.KInstantiateUI(ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject, parent, force_active: true);
		}
		RetiredColonyInfoScreen.Instance.Show();
		RetiredColonyInfoScreen.Instance.LoadColony(data);
	}

	private void SpawnVideoScreen()
	{
		VideoScreen.Instance = Util.KInstantiateUI(ScreenPrefabs.Instance.VideoScreen.gameObject, base.gameObject).GetComponent<VideoScreen>();
	}

	private void Update()
	{
	}

	public void RefreshResumeButton(bool simpleCheck = false)
	{
		string latestSaveForCurrentDLC = SaveLoader.GetLatestSaveForCurrentDLC();
		bool flag = !string.IsNullOrEmpty(latestSaveForCurrentDLC) && File.Exists(latestSaveForCurrentDLC);
		if (flag)
		{
			try
			{
				if (GenericGameSettings.instance.demoMode)
				{
					flag = false;
				}
				System.DateTime lastWriteTime = File.GetLastWriteTime(latestSaveForCurrentDLC);
				SaveFileEntry value = default(SaveFileEntry);
				SaveGame.Header header = default(SaveGame.Header);
				SaveGame.GameInfo gameInfo = default(SaveGame.GameInfo);
				if (!saveFileEntries.TryGetValue(latestSaveForCurrentDLC, out value) || value.timeStamp != lastWriteTime)
				{
					gameInfo = SaveLoader.LoadHeader(latestSaveForCurrentDLC, out header);
					SaveFileEntry saveFileEntry = default(SaveFileEntry);
					saveFileEntry.timeStamp = lastWriteTime;
					saveFileEntry.header = header;
					saveFileEntry.headerData = gameInfo;
					value = saveFileEntry;
					saveFileEntries[latestSaveForCurrentDLC] = value;
				}
				else
				{
					header = value.header;
					gameInfo = value.headerData;
				}
				if (header.buildVersion > 512719 || gameInfo.saveMajorVersion != 7 || gameInfo.saveMinorVersion > 28)
				{
					flag = false;
				}
				if (!DlcManager.IsContentActive(gameInfo.dlcId))
				{
					flag = false;
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(latestSaveForCurrentDLC);
				if (!string.IsNullOrEmpty(gameInfo.baseName))
				{
					Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = string.Format(UI.FRONTEND.MAINMENU.RESUMEBUTTON_BASENAME, gameInfo.baseName, gameInfo.numberOfCycles + 1);
				}
				else
				{
					Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = fileNameWithoutExtension;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex);
				flag = false;
			}
		}
		if (Button_ResumeGame != null && Button_ResumeGame.gameObject != null)
		{
			Button_ResumeGame.gameObject.SetActive(flag);
			KImage component = Button_NewGame.GetComponent<KImage>();
			component.colorStyleSetting = (flag ? normalButtonStyle : topButtonStyle);
			component.ApplyColorStyleSetting();
		}
		else
		{
			Debug.LogWarning("Why is the resume game button null?");
		}
	}

	private void Translations()
	{
		Util.KInstantiateUI<LanguageOptionsScreen>(ScreenPrefabs.Instance.languageOptionsScreen.gameObject, base.transform.parent.gameObject);
	}

	private void Mods()
	{
		Util.KInstantiateUI<ModsScreen>(ScreenPrefabs.Instance.modsMenu.gameObject, base.transform.parent.gameObject);
	}

	private void Options()
	{
		Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject, base.gameObject, force_active: true);
	}

	private void QuitGame()
	{
		App.Quit();
	}

	public void StartFEAudio()
	{
		AudioMixer.instance.Reset();
		MusicManager.instance.KillAllSongs(STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSnapshot);
		if (!AudioMixer.instance.SnapshotIsActive(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot))
		{
			AudioMixer.instance.StartUserVolumesSnapshot();
		}
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying(menuMusicEventName))
		{
			MusicManager.instance.PlaySong(menuMusicEventName);
		}
		CheckForAudioDriverIssue();
	}

	public void StopAmbience()
	{
		if (ambientLoop.isValid())
		{
			ambientLoop.stop(STOP_MODE.ALLOWFADEOUT);
			ambientLoop.release();
			ambientLoop.clearHandle();
		}
	}

	public void StopMainMenuMusic()
	{
		if (MusicManager.instance.SongIsPlaying(menuMusicEventName))
		{
			MusicManager.instance.StopSong(menuMusicEventName);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot);
		}
	}

	private void CheckForAudioDriverIssue()
	{
		if (!KFMOD.didFmodInitializeSuccessfully)
		{
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, force_active: true).PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS, null, null, UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS_MORE_INFO, delegate
			{
				App.OpenWebURL("http://support.kleientertainment.com/customer/en/portal/articles/2947881-no-audio-when-playing-oxygen-not-included");
			}, null, null, null, GlobalResources.Instance().sadDupeAudio);
		}
	}

	private void CheckPlayerPrefsCorruption()
	{
		if (KPlayerPrefs.HasCorruptedFlag())
		{
			KPlayerPrefs.ResetCorruptedFlag();
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, force_active: true).PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.PLAYER_PREFS_CORRUPTED, null, null, null, null, null, null, null, GlobalResources.Instance().sadDupe);
		}
	}

	private void CheckDoubleBoundKeys()
	{
		string text = "";
		HashSet<BindingEntry> hashSet = new HashSet<BindingEntry>();
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			if (GameInputMapping.KeyBindings[i].mKeyCode == KKeyCode.Mouse1)
			{
				continue;
			}
			for (int j = 0; j < GameInputMapping.KeyBindings.Length; j++)
			{
				if (i == j)
				{
					continue;
				}
				BindingEntry bindingEntry = GameInputMapping.KeyBindings[j];
				if (hashSet.Contains(bindingEntry))
				{
					continue;
				}
				BindingEntry bindingEntry2 = GameInputMapping.KeyBindings[i];
				if (bindingEntry2.mKeyCode != 0 && bindingEntry2.mKeyCode == bindingEntry.mKeyCode && bindingEntry2.mModifier == bindingEntry.mModifier && bindingEntry2.mRebindable && bindingEntry.mRebindable)
				{
					string mGroup = GameInputMapping.KeyBindings[i].mGroup;
					string mGroup2 = GameInputMapping.KeyBindings[j].mGroup;
					if ((mGroup == "Root" || mGroup2 == "Root" || mGroup == mGroup2) && (!(mGroup == "Root") || !bindingEntry.mIgnoreRootConflics) && (!(mGroup2 == "Root") || !bindingEntry2.mIgnoreRootConflics))
					{
						text = text + "\n\n" + bindingEntry2.mAction.ToString() + ": <b>" + bindingEntry2.mKeyCode.ToString() + "</b>\n" + bindingEntry.mAction.ToString() + ": <b>" + bindingEntry.mKeyCode.ToString() + "</b>";
						BindingEntry bindingEntry3 = bindingEntry2;
						bindingEntry3.mKeyCode = KKeyCode.None;
						bindingEntry3.mModifier = Modifier.None;
						GameInputMapping.KeyBindings[i] = bindingEntry3;
						bindingEntry3 = bindingEntry;
						bindingEntry3.mKeyCode = KKeyCode.None;
						bindingEntry3.mModifier = Modifier.None;
						GameInputMapping.KeyBindings[j] = bindingEntry3;
					}
				}
			}
			hashSet.Add(GameInputMapping.KeyBindings[i]);
		}
		if (text != "")
		{
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, force_active: true).PopupConfirmDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.DUPLICATE_KEY_BINDINGS, text), null, null, null, null, null, null, null, GlobalResources.Instance().sadDupe);
		}
	}

	private void RestartGame()
	{
		App.instance.Restart();
	}
}
