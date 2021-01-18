using System.IO;
using STRINGS;
using UnityEngine;

public class GameOptionsScreen : KModalButtonMenu
{
	[SerializeField]
	private SaveConfigurationScreen saveConfiguration;

	[SerializeField]
	private UnitConfigurationScreen unitConfiguration;

	[SerializeField]
	private GameObject expansion1ContentToggle;

	[SerializeField]
	private KButton resetTutorialButton;

	[SerializeField]
	private KButton controlsButton;

	[SerializeField]
	private KButton sandboxButton;

	[SerializeField]
	private ConfirmDialogScreen confirmPrefab;

	[SerializeField]
	private KButton doneButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private GameObject cloudSavesPanel;

	[SerializeField]
	private GameObject defaultToCloudSaveToggle;

	[SerializeField]
	private GameObject savePanel;

	[SerializeField]
	private InputBindingsScreen inputBindingsScreenPrefab;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		unitConfiguration.Init();
		if (SaveGame.Instance != null)
		{
			saveConfiguration.ToggleDisabledContent(enable: true);
			saveConfiguration.Init();
			SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
		}
		else
		{
			saveConfiguration.ToggleDisabledContent(enable: false);
		}
		if (expansion1ContentToggle != null)
		{
			expansion1ContentToggle.SetActive(DlcManager.IsExpansion1Installed());
			expansion1ContentToggle.GetComponentInChildren<ToolTip>().toolTip = UI.FRONTEND.GAME_OPTIONS_SCREEN.EXPANSION1_CONTENT_ENABLED_TOOLTIP;
			expansion1ContentToggle.GetComponentInChildren<KButton>().onClick += OnExpansion1ContentClicked;
			expansion1ContentToggle.GetComponentInChildren<LocText>().text = UI.FRONTEND.GAME_OPTIONS_SCREEN.EXPANSION1_CONTENT_ENABLED;
			UpdateExpansion1ContentToggle();
		}
		resetTutorialButton.onClick += OnTutorialReset;
		controlsButton.onClick += OnKeyBindings;
		sandboxButton.onClick += OnUnlockSandboxMode;
		doneButton.onClick += Deactivate;
		closeButton.onClick += Deactivate;
		if (defaultToCloudSaveToggle != null)
		{
			RefreshCloudSaveToggle();
			defaultToCloudSaveToggle.GetComponentInChildren<KButton>().onClick += OnDefaultToCloudSaveToggle;
		}
		if (cloudSavesPanel != null)
		{
			cloudSavesPanel.SetActive(SaveLoader.GetCloudSavesAvailable());
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (SaveGame.Instance != null)
		{
			savePanel.SetActive(value: true);
			saveConfiguration.Show(show);
			SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
		}
		else
		{
			savePanel.SetActive(value: false);
		}
	}

	private void OnDefaultToCloudSaveToggle()
	{
		bool cloudSavesDefault = SaveLoader.GetCloudSavesDefault();
		bool cloudSavesDefault2 = !cloudSavesDefault;
		SaveLoader.SetCloudSavesDefault(cloudSavesDefault2);
		RefreshCloudSaveToggle();
	}

	private void RefreshCloudSaveToggle()
	{
		bool cloudSavesDefault = SaveLoader.GetCloudSavesDefault();
		defaultToCloudSaveToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(cloudSavesDefault);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void OnTutorialReset()
	{
		ConfirmDialogScreen component = ActivateChildScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<ConfirmDialogScreen>();
		component.PopupConfirmDialog(UI.FRONTEND.OPTIONS_SCREEN.RESET_TUTORIAL_WARNING, delegate
		{
			Tutorial.ResetHiddenTutorialMessages();
		}, delegate
		{
		});
		component.Activate();
	}

	private void UpdateExpansion1ContentToggle()
	{
		bool active = DlcManager.IsExpansion1Active();
		expansion1ContentToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(active);
	}

	private void OnExpansion1ContentClicked()
	{
		Canvas componentInParent = GetComponentInParent<Canvas>();
		InfoDialogScreen infoDialogScreen = Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, componentInParent.gameObject).SetHeader(UI.FRONTEND.GAME_OPTIONS_SCREEN.EXPANSION1_CONTENT_TESTING_TITLE).AddPlainText(UI.FRONTEND.GAME_OPTIONS_SCREEN.EXPANSION1_CONTENT_TESTING_BODY)
			.AddDefaultOK();
		infoDialogScreen.gameObject.SetActive(value: true);
	}

	private void OnUnlockSandboxMode()
	{
		ConfirmDialogScreen component = ActivateChildScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<ConfirmDialogScreen>();
		component.PopupConfirmDialog(UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.UNLOCK_SANDBOX_WARNING, delegate
		{
			SaveGame.Instance.sandboxEnabled = true;
			SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
			TopLeftControlScreen.Instance.UpdateSandboxToggleState();
			Deactivate();
		}, delegate
		{
			string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
			string path = string.Concat(SaveGame.Instance.BaseName, UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.BACKUP_SAVE_GAME_APPEND, ".sav");
			SaveLoader.Instance.Save(Path.Combine(savePrefixAndCreateFolder, path), isAutoSave: false, updateSavePointer: false);
			SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
			TopLeftControlScreen.Instance.UpdateSandboxToggleState();
			Deactivate();
		}, confirm_text: UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM, cancel_text: UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM_SAVE_BACKUP, configurable_text: UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CANCEL, on_configurable_clicked: delegate
		{
		});
		component.Activate();
	}

	private void OnKeyBindings()
	{
		ActivateChildScreen(inputBindingsScreenPrefab.gameObject);
	}

	private void SetSandboxModeActive(bool active)
	{
		sandboxButton.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(active);
		sandboxButton.isInteractable = !active;
		sandboxButton.gameObject.GetComponentInParent<CanvasGroup>().alpha = (active ? 0.5f : 1f);
	}
}
