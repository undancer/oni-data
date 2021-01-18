using System.Collections;
using STRINGS;
using UnityEngine;

public class ValveSideScreen : SideScreenContent
{
	private Valve targetValve;

	[Header("Slider")]
	[SerializeField]
	private KSlider flowSlider;

	[SerializeField]
	private LocText minFlowLabel;

	[SerializeField]
	private LocText maxFlowLabel;

	[Header("Input Field")]
	[SerializeField]
	private KNumberInputField numberInput;

	[SerializeField]
	private LocText unitsLabel;

	private bool isEditing;

	private float targetFlow;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		unitsLabel.text = GameUtil.AddTimeSliceText(UI.UNITSUFFIXES.MASS.GRAM, GameUtil.TimeSlice.PerSecond);
		flowSlider.onReleaseHandle += OnReleaseHandle;
		flowSlider.onDrag += delegate
		{
			ReceiveValueFromSlider(flowSlider.value);
		};
		flowSlider.onPointerDown += delegate
		{
			ReceiveValueFromSlider(flowSlider.value);
		};
		flowSlider.onMove += delegate
		{
			ReceiveValueFromSlider(flowSlider.value);
			OnReleaseHandle();
		};
		numberInput.onEndEdit += delegate
		{
			ReceiveValueFromInput(numberInput.currentValue);
		};
		numberInput.decimalPlaces = 1;
	}

	public void OnReleaseHandle()
	{
		targetValve.ChangeFlow(targetFlow);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Valve>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		targetValve = target.GetComponent<Valve>();
		if (targetValve == null)
		{
			Debug.LogError("The target object does not have a Valve component.");
			return;
		}
		flowSlider.minValue = 0f;
		flowSlider.maxValue = targetValve.MaxFlow;
		flowSlider.value = targetValve.DesiredFlow;
		minFlowLabel.text = GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram);
		maxFlowLabel.text = GameUtil.GetFormattedMass(targetValve.MaxFlow, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram);
		numberInput.minValue = 0f;
		numberInput.maxValue = targetValve.MaxFlow * 1000f;
		numberInput.SetDisplayValue(GameUtil.GetFormattedMass(Mathf.Max(0f, targetValve.DesiredFlow), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, includeSuffix: false, "{0:0.#####}"));
		numberInput.Activate();
	}

	private void ReceiveValueFromSlider(float newValue)
	{
		newValue = Mathf.Round(newValue * 1000f) / 1000f;
		UpdateFlowValue(newValue);
	}

	private void ReceiveValueFromInput(float input)
	{
		float newValue = input / 1000f;
		UpdateFlowValue(newValue);
		targetValve.ChangeFlow(targetFlow);
	}

	private void UpdateFlowValue(float newValue)
	{
		targetFlow = newValue;
		flowSlider.value = newValue;
		numberInput.SetDisplayValue(GameUtil.GetFormattedMass(newValue, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, includeSuffix: false, "{0:0.#####}"));
	}

	private IEnumerator SettingDelay(float delay)
	{
		float startTime = Time.realtimeSinceStartup;
		float currentTime = startTime;
		while (currentTime < startTime + delay)
		{
			currentTime += Time.unscaledDeltaTime;
			yield return new WaitForEndOfFrame();
		}
		OnReleaseHandle();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		Debug.Log("ValveSideScreen OnKeyDown");
		if (isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}
}
