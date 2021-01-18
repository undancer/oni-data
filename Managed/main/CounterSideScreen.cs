using STRINGS;
using UnityEngine;

public class CounterSideScreen : SideScreenContent, IRender200ms
{
	public LogicCounter targetLogicCounter;

	public KButton resetButton;

	public KButton incrementMaxButton;

	public KButton decrementMaxButton;

	public KButton incrementModeButton;

	public KToggle advancedModeToggle;

	public KImage advancedModeCheckmark;

	public LocText currentCount;

	[SerializeField]
	private KNumberInputField maxCountInput;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		resetButton.onClick += ResetCounter;
		incrementMaxButton.onClick += IncrementMaxCount;
		decrementMaxButton.onClick += DecrementMaxCount;
		incrementModeButton.onClick += ToggleMode;
		advancedModeToggle.onClick += ToggleAdvanced;
		maxCountInput.onEndEdit += delegate
		{
			UpdateMaxCountFromTextInput(maxCountInput.currentValue);
		};
		UpdateCurrentCountLabel(targetLogicCounter.currentCount);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LogicCounter>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		maxCountInput.minValue = 1f;
		maxCountInput.maxValue = 10f;
		targetLogicCounter = target.GetComponent<LogicCounter>();
		UpdateCurrentCountLabel(targetLogicCounter.currentCount);
		UpdateMaxCountLabel(targetLogicCounter.maxCount);
		advancedModeCheckmark.enabled = targetLogicCounter.advancedMode;
	}

	public void Render200ms(float dt)
	{
		if (!(targetLogicCounter == null))
		{
			UpdateCurrentCountLabel(targetLogicCounter.currentCount);
		}
	}

	private void UpdateCurrentCountLabel(int value)
	{
		string text = value.ToString();
		text = ((value != targetLogicCounter.maxCount) ? UI.FormatAsAutomationState(text, UI.AutomationState.Standby) : UI.FormatAsAutomationState(text, UI.AutomationState.Active));
		currentCount.text = (targetLogicCounter.advancedMode ? string.Format(UI.UISIDESCREENS.COUNTER_SIDE_SCREEN.CURRENT_COUNT_ADVANCED, text) : string.Format(UI.UISIDESCREENS.COUNTER_SIDE_SCREEN.CURRENT_COUNT_SIMPLE, text));
	}

	private void UpdateMaxCountLabel(int value)
	{
		maxCountInput.SetAmount(value);
	}

	private void UpdateMaxCountFromTextInput(float newValue)
	{
		SetMaxCount((int)newValue);
	}

	private void IncrementMaxCount()
	{
		SetMaxCount(targetLogicCounter.maxCount + 1);
	}

	private void DecrementMaxCount()
	{
		SetMaxCount(targetLogicCounter.maxCount - 1);
	}

	private void SetMaxCount(int newValue)
	{
		if (newValue > 10)
		{
			newValue = 1;
		}
		if (newValue < 1)
		{
			newValue = 10;
		}
		if (newValue < targetLogicCounter.currentCount)
		{
			targetLogicCounter.currentCount = newValue;
		}
		targetLogicCounter.maxCount = newValue;
		UpdateCounterStates();
		UpdateMaxCountLabel(newValue);
	}

	private void ResetCounter()
	{
		targetLogicCounter.ResetCounter();
	}

	private void UpdateCounterStates()
	{
		targetLogicCounter.SetCounterState();
		targetLogicCounter.UpdateLogicCircuit();
		targetLogicCounter.UpdateVisualState(force: true);
		targetLogicCounter.UpdateMeter();
	}

	private void ToggleMode()
	{
	}

	private void ToggleAdvanced()
	{
		targetLogicCounter.advancedMode = !targetLogicCounter.advancedMode;
		advancedModeCheckmark.enabled = targetLogicCounter.advancedMode;
		UpdateCurrentCountLabel(targetLogicCounter.currentCount);
		UpdateCounterStates();
	}
}
