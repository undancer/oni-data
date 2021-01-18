using System;
using System.Collections.Generic;
using System.IO;
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

	public RectTransform LogoAndMenu;

	public KButton Button_ResumeGame;

	private KButton Button_NewGame;

	public GameObject topLeftAlphaMessage;

	private float lastUpdateTime;

	private MotdServerClient m_motdServerClient;

	private GameObject GameSettingsScreen;

	[SerializeField]
	private KButton buttonPrefab;

	[SerializeField]
	private GameObject buttonParent;

	[SerializeField]
	private ColorStyleSetting topButtonStyle;

	[SerializeField]
	private ColorStyleSetting normalButtonStyle;

	[SerializeField]
	private LocText motdImageHeader;

	[SerializeField]
	private Button motdImageButton;

	[SerializeField]
	private Image motdImage;

	[SerializeField]
	private LocText motdNewsHeader;

	[SerializeField]
	private LocText motdNewsBody;

	[SerializeField]
	private PatchNotesScreen patchNotesScreen;

	[SerializeField]
	private NextUpdateTimer nextUpdateTimer;

	[SerializeField]
	private DLCToggle expansion1Toggle;

	[SerializeField]
	private BuildWatermark buildWatermark;

	private static bool HasAutoresumedOnce = false;

	private bool refreshResumeButton = true;

	private int m_cheatInputCounter;

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
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		RefreshResumeButton();
		Button_ResumeGame.onClick += ResumeGame;
		StartFEAudio();
		SpawnVideoScreen();
		CheckPlayerPrefsCorruption();
		if (PatchNotesScreen.ShouldShowScreen())
		{
			patchNotesScreen.gameObject.SetActive(value: true);
		}
		CheckDoubleBoundKeys();
		topLeftAlphaMessage.gameObject.SetActive(value: false);
		nextUpdateTimer.gameObject.SetActive(value: false);
		expansion1Toggle.gameObject.SetActive(value: false);
		bool ownsExpansion1 = DistributionPlatform.Inst.PurchasedDLC;
		m_motdServerClient = new MotdServerClient();
		m_motdServerClient.GetMotd(delegate(MotdServerClient.MotdResponse response, string error)
		{
			if (error == null)
			{
				topLeftAlphaMessage.gameObject.SetActive(value: true);
				if (ownsExpansion1)
				{
					expansion1Toggle.gameObject.SetActive(value: true);
				}
				else
				{
					nextUpdateTimer.gameObject.SetActive(value: true);
				}
				motdImageHeader.text = response.image_header_text;
				motdNewsHeader.text = response.news_header_text;
				motdNewsBody.text = response.news_body_text;
				patchNotesScreen.UpdatePatchNotes(response.patch_notes_summary, response.patch_notes_link_url);
				nextUpdateTimer.UpdateReleaseTimes(response.last_update_time, response.next_update_time, response.update_text_override);
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
				motdImageButton.onClick.AddListener(delegate
				{
					Application.OpenURL(response.image_link_url);
				});
			}
			else
			{
				Debug.LogWarning("Motd Request error: " + error);
			}
		});
		lastUpdateTime = Time.unscaledTime;
		activateOnSpawn = true;
	}

	public void RefreshMainMenu()
	{
		if (refreshResumeButton)
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
		if ((GenericGameSettings.instance.autoResumeGame && !HasAutoresumedOnce) || !string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame))
		{
			HasAutoresumedOnce = true;
			ResumeGame();
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
		string text = (string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) ? SaveLoader.GetLatestSaveFile() : GenericGameSettings.instance.performanceCapture.saveGame);
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
			Util.KInstantiateUI(ScreenPrefabs.Instance.LoadScreen.gameObject, base.gameObject, force_active: true).GetComponent<LoadScreen>().requireConfirmation = false;
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
		if (Time.unscaledTime - lastUpdateTime > 1f)
		{
			RefreshMainMenu();
			lastUpdateTime = Time.unscaledTime;
		}
	}

	public void RefreshResumeButton()
	{
		string latestSaveFile = SaveLoader.GetLatestSaveFile();
		bool flag = !string.IsNullOrEmpty(latestSaveFile) && File.Exists(latestSaveFile);
		if (flag)
		{
			try
			{
				if (GenericGameSettings.instance.demoMode)
				{
					flag = false;
				}
				System.DateTime lastWriteTime = File.GetLastWriteTime(latestSaveFile);
				SaveFileEntry value = default(SaveFileEntry);
				SaveGame.Header header = default(SaveGame.Header);
				SaveGame.GameInfo gameInfo = default(SaveGame.GameInfo);
				if (!saveFileEntries.TryGetValue(latestSaveFile, out value) || value.timeStamp != lastWriteTime)
				{
					gameInfo = SaveLoader.LoadHeader(latestSaveFile, out header);
					SaveFileEntry saveFileEntry = default(SaveFileEntry);
					saveFileEntry.timeStamp = lastWriteTime;
					saveFileEntry.header = header;
					saveFileEntry.headerData = gameInfo;
					value = saveFileEntry;
					saveFileEntries[latestSaveFile] = value;
				}
				else
				{
					header = value.header;
					gameInfo = value.headerData;
				}
				if (header.buildVersion > 447596 || gameInfo.saveMajorVersion != 7 || gameInfo.saveMinorVersion > 17)
				{
					flag = false;
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(latestSaveFile);
				if (!string.IsNullOrEmpty(gameInfo.baseName))
				{
					Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = string.Format(UI.FRONTEND.MAINMENU.RESUMEBUTTON_BASENAME, gameInfo.baseName, gameInfo.numberOfCycles + 1);
				}
				else
				{
					Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = fileNameWithoutExtension;
				}
			}
			catch (Exception obj)
			{
				Debug.LogWarning(obj);
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
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSnapshot);
		if (!AudioMixer.instance.SnapshotIsActive(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot))
		{
			AudioMixer.instance.StartUserVolumesSnapshot();
		}
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying("Music_TitleTheme"))
		{
			MusicManager.instance.PlaySong("Music_TitleTheme");
		}
		CheckForAudioDriverIssue();
	}

	private void CheckForAudioDriverIssue()
	{
		if (!KFMOD.didFmodInitializeSuccessfully)
		{
			Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, force_active: true).PopupConfirmDialog(UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS, null, null, UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS_MORE_INFO, delegate
			{
				Application.OpenURL("http://support.kleientertainment.com/customer/en/portal/articles/2947881-no-audio-when-playing-oxygen-not-included");
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
						text = string.Concat(text, "\n\n", bindingEntry2.mAction, ": <b>", bindingEntry2.mKeyCode, "</b>\n", bindingEntry.mAction, ": <b>", bindingEntry.mKeyCode, "</b>");
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
