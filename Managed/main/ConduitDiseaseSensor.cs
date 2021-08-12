using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitDiseaseSensor : ConduitThresholdSensor, IThresholdSwitch
{
	private const float rangeMin = 0f;

	private const float rangeMax = 100000f;

	[Serialize]
	private float lastValue;

	private static readonly HashedString TINT_SYMBOL = "germs";

	public override float CurrentValue
	{
		get
		{
			GetContentsDisease(out var _, out var diseaseCount, out var hasMass);
			if (hasMass)
			{
				lastValue = diseaseCount;
			}
			return lastValue;
		}
	}

	public float RangeMin => 0f;

	public float RangeMax => 100000f;

	public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);

	protected override void UpdateVisualState(bool force = false)
	{
		if (!(wasOn != switchedOn || force))
		{
			return;
		}
		wasOn = switchedOn;
		if (switchedOn)
		{
			animController.Play(ConduitSensor.ON_ANIMS, KAnim.PlayMode.Loop);
			GetContentsDisease(out var diseaseIdx, out var _, out var _);
			Color32 color = Color.white;
			if (diseaseIdx != 255)
			{
				Disease disease = Db.Get().Diseases[diseaseIdx];
				color = GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName);
			}
			animController.SetSymbolTint(TINT_SYMBOL, color);
		}
		else
		{
			animController.Play(ConduitSensor.OFF_ANIMS);
		}
	}

	private void GetContentsDisease(out int diseaseIdx, out int diseaseCount, out bool hasMass)
	{
		int cell = Grid.PosToCell(this);
		if (conduitType == ConduitType.Liquid || conduitType == ConduitType.Gas)
		{
			ConduitFlow.ConduitContents contents = Conduit.GetFlowManager(conduitType).GetContents(cell);
			diseaseIdx = contents.diseaseIdx;
			diseaseCount = contents.diseaseCount;
			hasMass = contents.mass > 0f;
			return;
		}
		SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
		Pickupable pickupable = flowManager.GetPickupable(flowManager.GetContents(cell).pickupableHandle);
		if (pickupable != null && pickupable.PrimaryElement.Mass > 0f)
		{
			diseaseIdx = pickupable.PrimaryElement.DiseaseIdx;
			diseaseCount = pickupable.PrimaryElement.DiseaseCount;
			hasMass = true;
		}
		else
		{
			diseaseIdx = 0;
			diseaseCount = 0;
			hasMass = false;
		}
	}

	public float GetRangeMinInputField()
	{
		return 0f;
	}

	public float GetRangeMaxInputField()
	{
		return 100000f;
	}

	public string Format(float value, bool units)
	{
		return GameUtil.GetFormattedInt((int)value);
	}

	public float ProcessedSliderValue(float input)
	{
		return input;
	}

	public float ProcessedInputValue(float input)
	{
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_UNITS;
	}
}
