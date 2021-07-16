using System;
using Klei.CustomSettings;
using UnityEngine;

public class NewGameSettingToggle : NewGameSettingWidget
{
	[SerializeField]
	private LocText Label;

	[SerializeField]
	private ToolTip ToolTip;

	[SerializeField]
	private MultiToggle Toggle;

	[SerializeField]
	private ToolTip ToggleToolTip;

	private ToggleSettingConfig config;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle toggle = Toggle;
		toggle.onClick = (System.Action)Delegate.Combine(toggle.onClick, new System.Action(ToggleSetting));
	}

	public void Initialize(ToggleSettingConfig config, NewGameSettingsPanel panel, string disabledDefault)
	{
		base.Initialize(config, panel, disabledDefault);
		this.config = config;
		Label.text = config.label;
		ToolTip.toolTip = config.tooltip;
	}

	public override void Refresh()
	{
		base.Refresh();
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(config);
		Toggle.ChangeState(config.IsOnLevel(currentQualitySetting.id) ? 1 : 0);
		ToggleToolTip.toolTip = currentQualitySetting.tooltip;
	}

	public void ToggleSetting()
	{
		if (IsEnabled())
		{
			CustomGameSettings.Instance.ToggleSettingLevel(config);
			RefreshAll();
		}
	}
}
