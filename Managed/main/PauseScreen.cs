using System;
using System.IO;
using Klei;
using ProcGen;
using STRINGS;
using UnityEngine;

public class PauseScreen : KModalButtonMenu
{
	[SerializeField]
	private OptionsMenuScreen optionsScreen;

	[SerializeField]
	private SaveScreen saveScreenPrefab;

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private LocText worldSeed;

	[SerializeField]
	private CopyTextFieldToClipboard clipboard;

	private float originalTimeScale;

	private static PauseScreen instance;

	public static PauseScreen Instance => instance;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		keepMenuOpen = true;
		base.OnPrefabInit();
		if (!GenericGameSettings.instance.demoMode)
		{
			buttons = new ButtonInfo[8]
			{
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, Action.NumActions, OnResume),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVE, Action.NumActions, OnSave),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVEAS, Action.NumActions, OnSaveAs),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.LOAD, Action.NumActions, OnLoad),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, Action.NumActions, OnOptions),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.COLONY_SUMMARY, Action.NumActions, OnColonySummary),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, Action.NumActions, OnQuit),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, Action.NumActions, OnDesktopQuit)
			};
		}
		else
		{
			buttons = new ButtonInfo[4]
			{
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, Action.NumActions, OnResume),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, Action.NumActions, OnOptions),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, Action.NumActions, OnQuit),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, Action.NumActions, OnDesktopQuit)
			};
		}
		closeButton.onClick += OnResume;
		instance = this;
		Show(show: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		clipboard.GetText = GetClipboardText;
		title.SetText(UI.FRONTEND.PAUSE_SCREEN.TITLE);
		try
		{
			string settingsCoordinate = CustomGameSettings.Instance.GetSettingsCoordinate();
			string[] array = CustomGameSettings.ParseSettingCoordinate(settingsCoordinate);
			worldSeed.SetText(string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, settingsCoordinate));
			worldSeed.GetComponent<ToolTip>().toolTip = string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED_TOOLTIP, array[1], array[2], array[3]);
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"Failed to load Coordinates on ClusterLayout {arg}, please report this error on the forums");
			CustomGameSettings.Instance.Print();
			Debug.Log("ClusterCache: " + string.Join(",", SettingsCache.clusterLayouts.clusterCache.Keys));
			worldSeed.SetText(string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, "0"));
		}
	}

	private string GetClipboardText()
	{
		try
		{
			return CustomGameSettings.Instance.GetSettingsCoordinate();
		}
		catch
		{
			return "";
		}
	}

	private void OnResume()
	{
		Show(show: false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().ESCPauseSnapshot);
			MusicManager.instance.OnEscapeMenu(paused: true);
			MusicManager.instance.PlaySong("Music_ESC_Menu");
			return;
		}
		ToolTipScreen.Instance.ClearToolTip(closeButton.GetComponent<ToolTip>());
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().ESCPauseSnapshot);
		MusicManager.instance.OnEscapeMenu(paused: false);
		if (MusicManager.instance.SongIsPlaying("Music_ESC_Menu"))
		{
			MusicManager.instance.StopSong("Music_ESC_Menu");
		}
	}

	private void OnOptions()
	{
		ActivateChildScreen(optionsScreen.gameObject);
	}

	private void OnSaveAs()
	{
		ActivateChildScreen(saveScreenPrefab.gameObject);
	}

	private void OnSave()
	{
		string filename = SaveLoader.GetActiveSaveFilePath();
		if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
		{
			base.gameObject.SetActive(value: false);
			ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject);
			confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, System.IO.Path.GetFileNameWithoutExtension(filename)), delegate
			{
				DoSave(filename);
				base.gameObject.SetActive(value: true);
			}, OnCancelPopup);
		}
		else
		{
			OnSaveAs();
		}
	}

	private void DoSave(string filename)
	{
		try
		{
			SaveLoader.Instance.Save(filename);
		}
		catch (IOException ex)
		{
			IOException e = ex;
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, force_active: true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.IO_ERROR, e.ToString()), delegate
			{
				Deactivate();
			}, null, UI.FRONTEND.SAVESCREEN.REPORT_BUG, delegate
			{
				KCrashReporter.ReportError(e.Message, e.StackTrace.ToString(), null, null, null);
			});
		}
	}

	private void ConfirmDecision(string text, System.Action onConfirm)
	{
		base.gameObject.SetActive(value: false);
		ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject);
		confirmDialogScreen.PopupConfirmDialog(text, onConfirm, OnCancelPopup);
	}

	private void OnLoad()
	{
		ActivateChildScreen(loadScreenPrefab.gameObject);
	}

	private void OnColonySummary()
	{
		RetiredColonyData currentColonyRetiredColonyData = RetireColonyUtility.GetCurrentColonyRetiredColonyData();
		MainMenu.ActivateRetiredColoniesScreenFromData(Instance.transform.parent.gameObject, currentColonyRetiredColonyData);
	}

	private void OnQuit()
	{
		ConfirmDecision(UI.FRONTEND.MAINMENU.QUITCONFIRM, OnQuitConfirm);
	}

	private void OnDesktopQuit()
	{
		ConfirmDecision(UI.FRONTEND.MAINMENU.DESKTOPQUITCONFIRM, OnDesktopQuitConfirm);
	}

	private void OnCancelPopup()
	{
		base.gameObject.SetActive(value: true);
	}

	private void OnLoadConfirm()
	{
		LoadingOverlay.Load(delegate
		{
			LoadScreen.ForceStopGame();
			Deactivate();
			App.LoadScene("frontend");
		});
	}

	private void OnRetireConfirm()
	{
		RetireColonyUtility.SaveColonySummaryData();
	}

	private void OnQuitConfirm()
	{
		LoadingOverlay.Load(delegate
		{
			Deactivate();
			TriggerQuitGame();
		});
	}

	private void OnDesktopQuitConfirm()
	{
		App.Quit();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			Show(show: false);
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public static void TriggerQuitGame()
	{
		ThreadedHttps<KleiMetrics>.Instance.EndGame();
		LoadScreen.ForceStopGame();
		App.LoadScene("frontend");
	}
}
