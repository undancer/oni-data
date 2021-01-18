using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class TimerSideScreen : SideScreenContent, IRenderEveryTick
{
	public Image greenActiveZone;

	public Image redActiveZone;

	private LogicTimerSensor targetTimedSwitch;

	public KToggle modeButton;

	public KButton resetButton;

	public KSlider onDurationSlider;

	[SerializeField]
	private KNumberInputField onDurationNumberInput;

	public KSlider offDurationSlider;

	[SerializeField]
	private KNumberInputField offDurationNumberInput;

	public RectTransform endIndicator;

	public RectTransform currentTimeMarker;

	public LocText labelHeaderOnDuration;

	public LocText labelHeaderOffDuration;

	public LocText labelValueOnDuration;

	public LocText labelValueOffDuration;

	public LocText timeLeft;

	public float phaseLength;

	private bool cyclesMode;

	[SerializeField]
	private float minSeconds;

	[SerializeField]
	private float maxSeconds = 600f;

	[SerializeField]
	private float minCycles;

	[SerializeField]
	private float maxCycles = 10f;

	private const int CYCLEMODE_DECIMALS = 2;

	private const int SECONDSMODE_DECIMALS = 1;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		labelHeaderOnDuration.text = UI.UISIDESCREENS.TIMER_SIDE_SCREEN.ON;
		labelHeaderOffDuration.text = UI.UISIDESCREENS.TIMER_SIDE_SCREEN.OFF;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		modeButton.onClick += delegate
		{
			ToggleMode();
		};
		resetButton.onClick += ResetTimer;
		onDurationNumberInput.onEndEdit += delegate
		{
			UpdateDurationValueFromTextInput(onDurationNumberInput.currentValue, onDurationSlider);
		};
		offDurationNumberInput.onEndEdit += delegate
		{
			UpdateDurationValueFromTextInput(offDurationNumberInput.currentValue, offDurationSlider);
		};
		onDurationSlider.wholeNumbers = false;
		offDurationSlider.wholeNumbers = false;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LogicTimerSensor>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetTimedSwitch = target.GetComponent<LogicTimerSensor>();
		onDurationSlider.onValueChanged.RemoveAllListeners();
		offDurationSlider.onValueChanged.RemoveAllListeners();
		cyclesMode = targetTimedSwitch.displayCyclesMode;
		UpdateVisualsForNewTarget();
		ReconfigureRingVisuals();
		onDurationSlider.onValueChanged.AddListener(delegate
		{
			ChangeSetting();
		});
		offDurationSlider.onValueChanged.AddListener(delegate
		{
			ChangeSetting();
		});
	}

	private void UpdateVisualsForNewTarget()
	{
		float onDuration = targetTimedSwitch.onDuration;
		float offDuration = targetTimedSwitch.offDuration;
		bool displayCyclesMode = targetTimedSwitch.displayCyclesMode;
		if (displayCyclesMode)
		{
			onDurationSlider.minValue = minCycles;
			onDurationNumberInput.minValue = onDurationSlider.minValue;
			onDurationSlider.maxValue = maxCycles;
			onDurationNumberInput.maxValue = onDurationSlider.maxValue;
			onDurationNumberInput.decimalPlaces = 2;
			offDurationSlider.minValue = minCycles;
			offDurationNumberInput.minValue = offDurationSlider.minValue;
			offDurationSlider.maxValue = maxCycles;
			offDurationNumberInput.maxValue = offDurationSlider.maxValue;
			offDurationNumberInput.decimalPlaces = 2;
			onDurationSlider.value = onDuration / 600f;
			offDurationSlider.value = offDuration / 600f;
			onDurationNumberInput.SetAmount(onDuration / 600f);
			offDurationNumberInput.SetAmount(offDuration / 600f);
		}
		else
		{
			onDurationSlider.minValue = minSeconds;
			onDurationNumberInput.minValue = onDurationSlider.minValue;
			onDurationSlider.maxValue = maxSeconds;
			onDurationNumberInput.maxValue = onDurationSlider.maxValue;
			onDurationNumberInput.decimalPlaces = 1;
			offDurationSlider.minValue = minSeconds;
			offDurationNumberInput.minValue = offDurationSlider.minValue;
			offDurationSlider.maxValue = maxSeconds;
			offDurationNumberInput.maxValue = offDurationSlider.maxValue;
			offDurationNumberInput.decimalPlaces = 1;
			onDurationSlider.value = onDuration;
			offDurationSlider.value = offDuration;
			onDurationNumberInput.SetAmount(onDuration);
			offDurationNumberInput.SetAmount(offDuration);
		}
		modeButton.GetComponentInChildren<LocText>().text = (displayCyclesMode ? UI.UISIDESCREENS.TIMER_SIDE_SCREEN.MODE_LABEL_CYCLES : UI.UISIDESCREENS.TIMER_SIDE_SCREEN.MODE_LABEL_SECONDS);
	}

	private void ToggleMode()
	{
		cyclesMode = !cyclesMode;
		targetTimedSwitch.displayCyclesMode = cyclesMode;
		float value = onDurationSlider.value;
		float value2 = offDurationSlider.value;
		if (cyclesMode)
		{
			value = onDurationSlider.value / 600f;
			value2 = offDurationSlider.value / 600f;
		}
		else
		{
			value = onDurationSlider.value * 600f;
			value2 = offDurationSlider.value * 600f;
		}
		onDurationSlider.minValue = (cyclesMode ? minCycles : minSeconds);
		onDurationNumberInput.minValue = onDurationSlider.minValue;
		onDurationSlider.maxValue = (cyclesMode ? maxCycles : maxSeconds);
		onDurationNumberInput.maxValue = onDurationSlider.maxValue;
		onDurationNumberInput.decimalPlaces = ((!cyclesMode) ? 1 : 2);
		offDurationSlider.minValue = (cyclesMode ? minCycles : minSeconds);
		offDurationNumberInput.minValue = offDurationSlider.minValue;
		offDurationSlider.maxValue = (cyclesMode ? maxCycles : maxSeconds);
		offDurationNumberInput.maxValue = offDurationSlider.maxValue;
		offDurationNumberInput.decimalPlaces = ((!cyclesMode) ? 1 : 2);
		onDurationSlider.value = value;
		offDurationSlider.value = value2;
		onDurationNumberInput.SetAmount(value);
		offDurationNumberInput.SetAmount(value2);
		modeButton.GetComponentInChildren<LocText>().text = (cyclesMode ? UI.UISIDESCREENS.TIMER_SIDE_SCREEN.MODE_LABEL_CYCLES : UI.UISIDESCREENS.TIMER_SIDE_SCREEN.MODE_LABEL_SECONDS);
	}

	private void ChangeSetting()
	{
		targetTimedSwitch.onDuration = (cyclesMode ? (onDurationSlider.value * 600f) : onDurationSlider.value);
		targetTimedSwitch.offDuration = (cyclesMode ? (offDurationSlider.value * 600f) : offDurationSlider.value);
		ReconfigureRingVisuals();
		onDurationNumberInput.SetDisplayValue(cyclesMode ? (targetTimedSwitch.onDuration / 600f).ToString("F2") : targetTimedSwitch.onDuration.ToString());
		offDurationNumberInput.SetDisplayValue(cyclesMode ? (targetTimedSwitch.offDuration / 600f).ToString("F2") : targetTimedSwitch.offDuration.ToString());
		onDurationSlider.SetTooltipText(string.Format(UI.UISIDESCREENS.TIMER_SIDE_SCREEN.GREEN_DURATION_TOOLTIP, cyclesMode ? GameUtil.GetFormattedCycles(targetTimedSwitch.onDuration, "F2") : GameUtil.GetFormattedTime(targetTimedSwitch.onDuration)));
		offDurationSlider.SetTooltipText(string.Format(UI.UISIDESCREENS.TIMER_SIDE_SCREEN.RED_DURATION_TOOLTIP, cyclesMode ? GameUtil.GetFormattedCycles(targetTimedSwitch.offDuration, "F2") : GameUtil.GetFormattedTime(targetTimedSwitch.offDuration)));
		if (phaseLength == 0f)
		{
			timeLeft.text = UI.UISIDESCREENS.TIMER_SIDE_SCREEN.DISABLED;
			if (targetTimedSwitch.IsSwitchedOn)
			{
				greenActiveZone.fillAmount = 1f;
				redActiveZone.fillAmount = 0f;
			}
			else
			{
				greenActiveZone.fillAmount = 0f;
				redActiveZone.fillAmount = 1f;
			}
			targetTimedSwitch.timeElapsedInCurrentState = 0f;
			currentTimeMarker.rotation = Quaternion.identity;
			currentTimeMarker.Rotate(0f, 0f, 0f);
		}
	}

	private void ReconfigureRingVisuals()
	{
		phaseLength = targetTimedSwitch.onDuration + targetTimedSwitch.offDuration;
		greenActiveZone.fillAmount = targetTimedSwitch.onDuration / phaseLength;
		redActiveZone.fillAmount = targetTimedSwitch.offDuration / phaseLength;
	}

	public void RenderEveryTick(float dt)
	{
		if (phaseLength != 0f)
		{
			float timeElapsedInCurrentState = targetTimedSwitch.timeElapsedInCurrentState;
			if (cyclesMode)
			{
				timeLeft.text = string.Format(UI.UISIDESCREENS.TIMER_SIDE_SCREEN.CURRENT_TIME, GameUtil.GetFormattedCycles(timeElapsedInCurrentState, "F2"), GameUtil.GetFormattedCycles(targetTimedSwitch.IsSwitchedOn ? targetTimedSwitch.onDuration : targetTimedSwitch.offDuration, "F2"));
			}
			else
			{
				timeLeft.text = string.Format(UI.UISIDESCREENS.TIMER_SIDE_SCREEN.CURRENT_TIME, GameUtil.GetFormattedTime(timeElapsedInCurrentState, "F1"), GameUtil.GetFormattedTime(targetTimedSwitch.IsSwitchedOn ? targetTimedSwitch.onDuration : targetTimedSwitch.offDuration, "F1"));
			}
			currentTimeMarker.rotation = Quaternion.identity;
			if (targetTimedSwitch.IsSwitchedOn)
			{
				currentTimeMarker.Rotate(0f, 0f, targetTimedSwitch.timeElapsedInCurrentState / phaseLength * -360f);
			}
			else
			{
				currentTimeMarker.Rotate(0f, 0f, (targetTimedSwitch.onDuration + targetTimedSwitch.timeElapsedInCurrentState) / phaseLength * -360f);
			}
		}
	}

	private void UpdateDurationValueFromTextInput(float newValue, KSlider slider)
	{
		if (newValue < slider.minValue)
		{
			newValue = slider.minValue;
		}
		if (newValue > slider.maxValue)
		{
			newValue = slider.maxValue;
		}
		slider.value = newValue;
		NonLinearSlider nonLinearSlider = slider as NonLinearSlider;
		if (nonLinearSlider != null)
		{
			slider.value = nonLinearSlider.GetPercentageFromValue(newValue);
		}
		else
		{
			slider.value = newValue;
		}
	}

	private void ResetTimer()
	{
		targetTimedSwitch.ResetTimer();
	}
}
