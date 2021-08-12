using Klei.CustomSettings;
using UnityEngine;
using UnityEngine.UI;

public abstract class NewGameSettingWidget : KMonoBehaviour
{
	[SerializeField]
	private Image BG;

	[SerializeField]
	private Color enabledColor;

	[SerializeField]
	private Color disabledColor;

	private SettingConfig config;

	private NewGameSettingsPanel panel;

	private string disabledDefault;

	private bool widget_enabled = true;

	protected virtual void Initialize(SettingConfig config, NewGameSettingsPanel panel, string disabledDefault)
	{
		this.config = config;
		this.panel = panel;
		this.disabledDefault = disabledDefault;
	}

	public virtual void Refresh()
	{
		bool flag = ShouldBeEnabled();
		if (flag != widget_enabled)
		{
			widget_enabled = flag;
			if (IsEnabled())
			{
				BG.color = enabledColor;
				CustomGameSettings.Instance.SetQualitySetting(config, config.GetDefaultLevelId());
			}
			else
			{
				CustomGameSettings.Instance.SetQualitySetting(config, disabledDefault);
				BG.color = disabledColor;
			}
		}
	}

	protected void RefreshAll()
	{
		panel.Refresh();
	}

	protected bool IsEnabled()
	{
		return widget_enabled;
	}

	private bool ShouldBeEnabled()
	{
		return true;
	}
}
