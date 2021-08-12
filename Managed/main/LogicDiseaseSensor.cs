using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicDiseaseSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
	[SerializeField]
	[Serialize]
	private float threshold;

	[SerializeField]
	[Serialize]
	private bool activateAboveThreshold = true;

	private KBatchedAnimController animController;

	private bool wasOn;

	private const float rangeMin = 0f;

	private const float rangeMax = 100000f;

	private const int WINDOW_SIZE = 8;

	private int[] samples = new int[8];

	private int sampleIdx;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicDiseaseSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicDiseaseSensor>(delegate(LogicDiseaseSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly HashedString[] ON_ANIMS = new HashedString[2] { "on_pre", "on_loop" };

	private static readonly HashedString[] OFF_ANIMS = new HashedString[2] { "on_pst", "off" };

	private static readonly HashedString TINT_SYMBOL = "germs";

	public float Threshold
	{
		get
		{
			return threshold;
		}
		set
		{
			threshold = value;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateAboveThreshold;
		}
		set
		{
			activateAboveThreshold = value;
		}
	}

	public float CurrentValue
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < 8; i++)
			{
				num += (float)samples[i];
			}
			return num / 8f;
		}
	}

	public float RangeMin => 0f;

	public float RangeMax => 100000f;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 100;

	public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);

	public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicDiseaseSensor component = ((GameObject)data).GetComponent<LogicDiseaseSensor>();
		if (component != null)
		{
			Threshold = component.Threshold;
			ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		animController = GetComponent<KBatchedAnimController>();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(force: true);
		wasOn = switchedOn;
	}

	public void Sim200ms(float dt)
	{
		if (sampleIdx < 8)
		{
			int i = Grid.PosToCell(this);
			if (Grid.Mass[i] > 0f)
			{
				samples[sampleIdx] = Grid.DiseaseCount[i];
				sampleIdx++;
			}
			return;
		}
		sampleIdx = 0;
		float currentValue = CurrentValue;
		if (activateAboveThreshold)
		{
			if ((currentValue > threshold && !base.IsSwitchedOn) || (currentValue <= threshold && base.IsSwitchedOn))
			{
				Toggle();
			}
		}
		else if ((currentValue > threshold && base.IsSwitchedOn) || (currentValue <= threshold && !base.IsSwitchedOn))
		{
			Toggle();
		}
		animController.SetSymbolVisiblity(TINT_SYMBOL, currentValue > 0f);
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
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

	private void UpdateLogicCircuit()
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (!(wasOn != switchedOn || force))
		{
			return;
		}
		wasOn = switchedOn;
		if (switchedOn)
		{
			animController.Play(ON_ANIMS, KAnim.PlayMode.Loop);
			int i = Grid.PosToCell(this);
			byte b = Grid.DiseaseIdx[i];
			Color32 color = Color.white;
			if (b != byte.MaxValue)
			{
				Disease disease = Db.Get().Diseases[b];
				color = GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName);
			}
			animController.SetSymbolTint(TINT_SYMBOL, color);
		}
		else
		{
			animController.Play(OFF_ANIMS);
		}
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}
}
