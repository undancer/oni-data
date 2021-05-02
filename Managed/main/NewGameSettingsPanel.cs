using System;
using System.Collections.Generic;
using Klei.CustomSettings;
using KMod;
using ProcGen;
using ProcGenGame;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NewGameSettingsPanel")]
public class NewGameSettingsPanel : KMonoBehaviour
{
	[SerializeField]
	private Transform content;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton background;

	[Header("Prefab UI Refs")]
	[SerializeField]
	private GameObject prefab_cycle_setting;

	[SerializeField]
	private GameObject prefab_slider_setting;

	[SerializeField]
	private GameObject prefab_checkbox_setting;

	[SerializeField]
	private GameObject prefab_seed_input_setting;

	private CustomGameSettings settings;

	private List<NewGameSettingWidget> widgets;

	public void SetCloseAction(System.Action onClose)
	{
		if (closeButton != null)
		{
			closeButton.onClick += onClose;
		}
		if (background != null)
		{
			background.onClick += onClose;
		}
	}

	public void Init()
	{
		Global.Instance.modManager.Load(Content.LayerableFiles);
		SettingsCache.Clear();
		WorldGen.LoadSettings();
		CustomGameSettings.Instance.LoadWorlds();
		CustomGameSettings.Instance.LoadClusters();
		Global.Instance.modManager.Report(base.gameObject);
		settings = CustomGameSettings.Instance;
		widgets = new List<NewGameSettingWidget>();
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in settings.QualitySettings)
		{
			if ((qualitySetting.Value.debug_only && !DebugHandler.enabled) || (qualitySetting.Value.editor_only && !Application.isEditor) || !DlcManager.IsContentActive(qualitySetting.Value.required_content))
			{
				continue;
			}
			ListSettingConfig listSettingConfig = qualitySetting.Value as ListSettingConfig;
			if (listSettingConfig != null)
			{
				NewGameSettingList newGameSettingList = Util.KInstantiateUI<NewGameSettingList>(prefab_cycle_setting, content.gameObject, force_active: true);
				newGameSettingList.Initialize(listSettingConfig, this, qualitySetting.Value.missing_content_default);
				widgets.Add(newGameSettingList);
				continue;
			}
			ToggleSettingConfig toggleSettingConfig = qualitySetting.Value as ToggleSettingConfig;
			if (toggleSettingConfig != null)
			{
				NewGameSettingToggle newGameSettingToggle = Util.KInstantiateUI<NewGameSettingToggle>(prefab_checkbox_setting, content.gameObject, force_active: true);
				newGameSettingToggle.Initialize(toggleSettingConfig, this, qualitySetting.Value.missing_content_default);
				widgets.Add(newGameSettingToggle);
				continue;
			}
			SeedSettingConfig seedSettingConfig = qualitySetting.Value as SeedSettingConfig;
			if (seedSettingConfig != null)
			{
				NewGameSettingSeed newGameSettingSeed = Util.KInstantiateUI<NewGameSettingSeed>(prefab_seed_input_setting, content.gameObject, force_active: true);
				newGameSettingSeed.Initialize(seedSettingConfig);
				widgets.Add(newGameSettingSeed);
			}
		}
		Refresh();
	}

	public void Refresh()
	{
		foreach (NewGameSettingWidget widget in widgets)
		{
			widget.Refresh();
		}
	}

	public void ConsumeSettingsCode(string code)
	{
		settings.ParseAndApplySettingsCode(code);
	}

	public void SetSetting(SettingConfig setting, string level)
	{
		settings.SetQualitySetting(setting, level);
	}

	public string GetSetting(SettingConfig setting)
	{
		return settings.GetCurrentQualitySetting(setting).id;
	}

	public void Cancel()
	{
		Global.Instance.modManager.Unload(Content.LayerableFiles);
		SettingsCache.Clear();
	}
}
