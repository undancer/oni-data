using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureSwitchSideScreen : SideScreenContent, IRender200ms
{
	private TemperatureControlledSwitch targetTemperatureSwitch;

	[SerializeField]
	private LocText currentTemperature;

	[SerializeField]
	private LocText targetTemperature;

	[SerializeField]
	private KToggle coolerToggle;

	[SerializeField]
	private KToggle warmerToggle;

	[SerializeField]
	private KSlider targetTemperatureSlider;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		coolerToggle.onClick += delegate
		{
			OnConditionButtonClicked(isWarmer: false);
		};
		warmerToggle.onClick += delegate
		{
			OnConditionButtonClicked(isWarmer: true);
		};
		LocText component = coolerToggle.transform.GetChild(0).GetComponent<LocText>();
		LocText component2 = warmerToggle.transform.GetChild(0).GetComponent<LocText>();
		component.SetText(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.COLDER_BUTTON);
		component2.SetText(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.WARMER_BUTTON);
		Slider.SliderEvent sliderEvent = new Slider.SliderEvent();
		sliderEvent.AddListener(OnTargetTemperatureChanged);
		targetTemperatureSlider.onValueChanged = sliderEvent;
	}

	public void Render200ms(float dt)
	{
		if (!(targetTemperatureSwitch == null))
		{
			UpdateLabels();
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<TemperatureControlledSwitch>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		if (target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		targetTemperatureSwitch = target.GetComponent<TemperatureControlledSwitch>();
		if (targetTemperatureSwitch == null)
		{
			Debug.LogError("The gameObject received does not contain a TimedSwitch component");
			return;
		}
		UpdateLabels();
		UpdateTargetTemperatureLabel();
		OnConditionButtonClicked(targetTemperatureSwitch.activateOnWarmerThan);
	}

	private void OnTargetTemperatureChanged(float new_value)
	{
		targetTemperatureSwitch.thresholdTemperature = new_value;
		UpdateTargetTemperatureLabel();
	}

	private void OnConditionButtonClicked(bool isWarmer)
	{
		targetTemperatureSwitch.activateOnWarmerThan = isWarmer;
		if (isWarmer)
		{
			coolerToggle.isOn = false;
			warmerToggle.isOn = true;
			coolerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
			warmerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
		}
		else
		{
			coolerToggle.isOn = true;
			warmerToggle.isOn = false;
			coolerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Active);
			warmerToggle.GetComponent<ImageToggleState>().SetState(ImageToggleState.State.Inactive);
		}
	}

	private void UpdateTargetTemperatureLabel()
	{
		targetTemperature.text = GameUtil.GetFormattedTemperature(targetTemperatureSwitch.thresholdTemperature);
	}

	private void UpdateLabels()
	{
		currentTemperature.text = string.Format(UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.CURRENT_TEMPERATURE, GameUtil.GetFormattedTemperature(targetTemperatureSwitch.GetTemperature()));
	}
}
