using STRINGS;
using UnityEngine;

public class LimitValveSideScreen : SideScreenContent
{
	public static readonly string FLOAT_FORMAT = "{0:0.#####}";

	private LimitValve targetLimitValve;

	[Header("State")]
	[SerializeField]
	private LocText amountLabel;

	[SerializeField]
	private KButton resetButton;

	[Header("Slider")]
	[SerializeField]
	private NonLinearSlider limitSlider;

	[SerializeField]
	private LocText minLimitLabel;

	[SerializeField]
	private LocText maxLimitLabel;

	[SerializeField]
	private ToolTip toolTip;

	[Header("Input Field")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	private float targetLimit;

	private int targetLimitValveSubHandle = -1;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		resetButton.onClick += ResetCounter;
		limitSlider.onReleaseHandle += OnReleaseHandle;
		limitSlider.onDrag += delegate
		{
			ReceiveValueFromSlider(limitSlider.value);
		};
		limitSlider.onPointerDown += delegate
		{
			ReceiveValueFromSlider(limitSlider.value);
		};
		limitSlider.onMove += delegate
		{
			ReceiveValueFromSlider(limitSlider.value);
			OnReleaseHandle();
		};
		numberInput.onEndEdit += delegate
		{
			ReceiveValueFromInput(numberInput.currentValue);
		};
		numberInput.decimalPlaces = 3;
	}

	public void OnReleaseHandle()
	{
		targetLimitValve.Limit = targetLimit;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LimitValve>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		targetLimitValve = target.GetComponent<LimitValve>();
		if (targetLimitValve == null)
		{
			Debug.LogError("The target object does not have a LimitValve component.");
			return;
		}
		if (targetLimitValveSubHandle != -1)
		{
			Unsubscribe(targetLimitValveSubHandle);
		}
		targetLimitValveSubHandle = targetLimitValve.Subscribe(-1722241721, UpdateAmountLabel);
		limitSlider.minValue = 0f;
		limitSlider.maxValue = 100f;
		limitSlider.SetRanges(targetLimitValve.GetRanges());
		limitSlider.value = limitSlider.GetPercentageFromValue(targetLimitValve.Limit);
		numberInput.minValue = 0f;
		numberInput.maxValue = targetLimitValve.maxLimitKg;
		numberInput.Activate();
		if (targetLimitValve.displayUnitsInsteadOfMass)
		{
			minLimitLabel.text = GameUtil.GetFormattedUnits(0f);
			maxLimitLabel.text = GameUtil.GetFormattedUnits(targetLimitValve.maxLimitKg);
			numberInput.SetDisplayValue(GameUtil.GetFormattedUnits(Mathf.Max(0f, targetLimitValve.Limit), GameUtil.TimeSlice.None, displaySuffix: false, FLOAT_FORMAT));
			unitsLabel.text = UI.UNITSUFFIXES.UNITS;
			toolTip.enabled = true;
			toolTip.SetSimpleTooltip(UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.SLIDER_TOOLTIP_UNITS);
		}
		else
		{
			minLimitLabel.text = GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram);
			maxLimitLabel.text = GameUtil.GetFormattedMass(targetLimitValve.maxLimitKg, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram);
			numberInput.SetDisplayValue(GameUtil.GetFormattedMass(Mathf.Max(0f, targetLimitValve.Limit), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, includeSuffix: false, FLOAT_FORMAT));
			unitsLabel.text = GameUtil.GetCurrentMassUnit();
			toolTip.enabled = false;
		}
		UpdateAmountLabel();
	}

	private void UpdateAmountLabel(object obj = null)
	{
		if (targetLimitValve.displayUnitsInsteadOfMass)
		{
			string formattedUnits = GameUtil.GetFormattedUnits(targetLimitValve.Amount, GameUtil.TimeSlice.None, displaySuffix: true, FLOAT_FORMAT);
			amountLabel.text = string.Format(UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.AMOUNT, formattedUnits);
		}
		else
		{
			string formattedMass = GameUtil.GetFormattedMass(targetLimitValve.Amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, includeSuffix: true, FLOAT_FORMAT);
			amountLabel.text = string.Format(UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.AMOUNT, formattedMass);
		}
	}

	private void ResetCounter()
	{
		targetLimitValve.ResetAmount();
	}

	private void ReceiveValueFromSlider(float sliderPercentage)
	{
		float valueForPercentage = limitSlider.GetValueForPercentage(sliderPercentage);
		valueForPercentage = Mathf.RoundToInt(valueForPercentage);
		UpdateLimitValue(valueForPercentage);
	}

	private void ReceiveValueFromInput(float input)
	{
		UpdateLimitValue(input);
		targetLimitValve.Limit = targetLimit;
	}

	private void UpdateLimitValue(float newValue)
	{
		targetLimit = newValue;
		limitSlider.value = limitSlider.GetPercentageFromValue(newValue);
		if (targetLimitValve.displayUnitsInsteadOfMass)
		{
			numberInput.SetDisplayValue(GameUtil.GetFormattedUnits(newValue, GameUtil.TimeSlice.None, displaySuffix: false, FLOAT_FORMAT));
		}
		else
		{
			numberInput.SetDisplayValue(GameUtil.GetFormattedMass(newValue, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, includeSuffix: false, FLOAT_FORMAT));
		}
	}
}
