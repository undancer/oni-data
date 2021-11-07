using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicHEPSensor : Switch, ISaveLoadable, IThresholdSwitch, ISimEveryTick
{
	[Serialize]
	public float thresholdPayload;

	[Serialize]
	public bool activateOnHigherThan;

	[Serialize]
	public bool dirty = true;

	private readonly float minPayload;

	private readonly float maxPayload = 500f;

	private float foundPayload;

	private bool waitForLogicTick;

	private bool wasOn;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicHEPSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicHEPSensor>(delegate(LogicHEPSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public float Threshold
	{
		get
		{
			return thresholdPayload;
		}
		set
		{
			thresholdPayload = value;
			dirty = true;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateOnHigherThan;
		}
		set
		{
			activateOnHigherThan = value;
			dirty = true;
		}
	}

	public float CurrentValue => foundPayload;

	public float RangeMin => minPayload;

	public float RangeMax => maxPayload;

	public LocString Title => UI.UISIDESCREENS.HEPSWITCHSIDESCREEN.TITLE;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.HEPS;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.HEPS_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.HEPS_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[3]
	{
		new NonLinearSlider.Range(30f, 50f),
		new NonLinearSlider.Range(30f, 200f),
		new NonLinearSlider.Range(40f, 500f)
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicHEPSensor component = ((GameObject)data).GetComponent<LogicHEPSensor>();
		if (component != null)
		{
			Threshold = component.Threshold;
			ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += OnSwitchToggled;
		UpdateLogicCircuit();
		UpdateVisualState(force: true);
		wasOn = switchedOn;
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		logicCircuitManager.onLogicTick = (System.Action)Delegate.Combine(logicCircuitManager.onLogicTick, new System.Action(LogicTick));
	}

	protected override void OnCleanUp()
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		logicCircuitManager.onLogicTick = (System.Action)Delegate.Remove(logicCircuitManager.onLogicTick, new System.Action(LogicTick));
		base.OnCleanUp();
	}

	public void SimEveryTick(float dt)
	{
		if (waitForLogicTick)
		{
			return;
		}
		Vector2I vector2I = Grid.CellToXY(Grid.PosToCell(this));
		ListPool<ScenePartitionerEntry, LogicHEPSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicHEPSensor>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
		float num = 0f;
		foreach (ScenePartitionerEntry item in pooledList)
		{
			HighEnergyParticle component = (item.obj as KCollider2D).gameObject.GetComponent<HighEnergyParticle>();
			if (!(component == null) && component.isCollideable)
			{
				num += component.payload;
			}
		}
		pooledList.Recycle();
		foundPayload = num;
		bool flag = (activateOnHigherThan && num > thresholdPayload) || (!activateOnHigherThan && num < thresholdPayload);
		if (flag != switchedOn)
		{
			waitForLogicTick = true;
		}
		SetState(flag);
	}

	private void LogicTick()
	{
		waitForLogicTick = false;
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		UpdateLogicCircuit();
		UpdateVisualState();
	}

	private void UpdateLogicCircuit()
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (wasOn != switchedOn || force)
		{
			wasOn = switchedOn;
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.Play(switchedOn ? "on_pre" : "on_pst");
			component.Queue(switchedOn ? "on" : "off");
		}
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}

	public float GetRangeMinInputField()
	{
		return minPayload;
	}

	public float GetRangeMaxInputField()
	{
		return maxPayload;
	}

	public string Format(float value, bool units)
	{
		return GameUtil.GetFormattedHighEnergyParticles(value, GameUtil.TimeSlice.None, units);
	}

	public float ProcessedSliderValue(float input)
	{
		return Mathf.Round(input);
	}

	public float ProcessedInputValue(float input)
	{
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;
	}
}
