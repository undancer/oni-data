using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicMassSensor : Switch, ISaveLoadable, IThresholdSwitch
{
	[SerializeField]
	[Serialize]
	private float threshold;

	[SerializeField]
	[Serialize]
	private bool activateAboveThreshold = true;

	[MyCmpGet]
	private LogicPorts logicPorts;

	private bool was_pressed;

	private bool was_on;

	public float rangeMin;

	public float rangeMax = 1f;

	[Serialize]
	private float massSolid;

	[Serialize]
	private float massPickupables;

	[Serialize]
	private float massActivators;

	private const float MIN_TOGGLE_TIME = 0.15f;

	private float toggleCooldown = 0.15f;

	private HandleVector<int>.Handle solidChangedEntry;

	private HandleVector<int>.Handle pickupablesChangedEntry;

	private HandleVector<int>.Handle floorSwitchActivatorChangedEntry;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicMassSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicMassSensor>(delegate(LogicMassSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;

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

	public float CurrentValue => massSolid + massPickupables + massActivators;

	public float RangeMin => rangeMin;

	public float RangeMax => rangeMax;

	public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;

	public string AboveToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_ABOVE;

	public string BelowToolTip => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_BELOW;

	public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

	public int IncrementScale => 1;

	public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicMassSensor component = ((GameObject)data).GetComponent<LogicMassSensor>();
		if (component != null)
		{
			Threshold = component.Threshold;
			ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateVisualState(force: true);
		int cell = Grid.CellAbove(this.NaturalBuildingCell());
		solidChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.SolidChanged", base.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		pickupablesChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.PickupablesChanged", base.gameObject, cell, GameScenePartitioner.Instance.pickupablesChangedLayer, OnPickupablesChanged);
		floorSwitchActivatorChangedEntry = GameScenePartitioner.Instance.Add("LogicMassSensor.SwitchActivatorChanged", base.gameObject, cell, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, OnActivatorsChanged);
		base.OnToggle += SwitchToggled;
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref solidChangedEntry);
		GameScenePartitioner.Instance.Free(ref pickupablesChangedEntry);
		GameScenePartitioner.Instance.Free(ref floorSwitchActivatorChangedEntry);
		base.OnCleanUp();
	}

	private void Update()
	{
		toggleCooldown = Mathf.Max(0f, toggleCooldown - Time.deltaTime);
		if (toggleCooldown == 0f)
		{
			float currentValue = CurrentValue;
			if ((activateAboveThreshold ? (currentValue > threshold) : (currentValue < threshold)) != base.IsSwitchedOn)
			{
				Toggle();
				toggleCooldown = 0.15f;
			}
			UpdateVisualState();
		}
	}

	private void OnSolidChanged(object data)
	{
		int i = Grid.CellAbove(this.NaturalBuildingCell());
		if (Grid.Solid[i])
		{
			massSolid = Grid.Mass[i];
		}
		else
		{
			massSolid = 0f;
		}
	}

	private void OnPickupablesChanged(object data)
	{
		float num = 0f;
		int cell = Grid.CellAbove(this.NaturalBuildingCell());
		ListPool<ScenePartitionerEntry, LogicMassSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicMassSensor>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(cell).x, Grid.CellToXY(cell).y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			Pickupable pickupable = pooledList[i].obj as Pickupable;
			if (!(pickupable == null) && !pickupable.wasAbsorbed)
			{
				KPrefabID component = pickupable.GetComponent<KPrefabID>();
				if (!component.HasTag(GameTags.Creature) || component.HasTag(GameTags.Creatures.Walker) || component.HasTag(GameTags.Creatures.Hoverer) || pickupable.HasTag(GameTags.Creatures.Flopping))
				{
					num += pickupable.PrimaryElement.Mass;
				}
			}
		}
		pooledList.Recycle();
		massPickupables = num;
	}

	private void OnActivatorsChanged(object data)
	{
		float num = 0f;
		int cell = Grid.CellAbove(this.NaturalBuildingCell());
		ListPool<ScenePartitionerEntry, LogicMassSensor>.PooledList pooledList = ListPool<ScenePartitionerEntry, LogicMassSensor>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(cell).x, Grid.CellToXY(cell).y, 1, 1, GameScenePartitioner.Instance.floorSwitchActivatorLayer, pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			FloorSwitchActivator floorSwitchActivator = pooledList[i].obj as FloorSwitchActivator;
			if (!(floorSwitchActivator == null))
			{
				num += floorSwitchActivator.PrimaryElement.Mass;
			}
		}
		pooledList.Recycle();
		massActivators = num;
	}

	public float GetRangeMinInputField()
	{
		return rangeMin;
	}

	public float GetRangeMaxInputField()
	{
		return rangeMax;
	}

	public string Format(float value, bool units)
	{
		GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.Kilogram;
		bool includeSuffix = units;
		return GameUtil.GetFormattedMass(value, GameUtil.TimeSlice.None, massFormat, includeSuffix);
	}

	public float ProcessedSliderValue(float input)
	{
		input = Mathf.Round(input);
		return input;
	}

	public float ProcessedInputValue(float input)
	{
		return input;
	}

	public LocString ThresholdValueUnits()
	{
		return GameUtil.GetCurrentMassUnit();
	}

	private void SwitchToggled(bool toggled_on)
	{
		GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, toggled_on ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		bool flag = CurrentValue > threshold;
		if (!(flag != was_pressed || was_on != base.IsSwitchedOn || force))
		{
			return;
		}
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		if (flag)
		{
			if (force)
			{
				component.Play(base.IsSwitchedOn ? "on_down" : "off_down");
			}
			else
			{
				component.Play(base.IsSwitchedOn ? "on_down_pre" : "off_down_pre");
				component.Queue(base.IsSwitchedOn ? "on_down" : "off_down");
			}
		}
		else if (force)
		{
			component.Play(base.IsSwitchedOn ? "on_up" : "off_up");
		}
		else
		{
			component.Play(base.IsSwitchedOn ? "on_up_pre" : "off_up_pre");
			component.Queue(base.IsSwitchedOn ? "on_up" : "off_up");
		}
		was_pressed = flag;
		was_on = base.IsSwitchedOn;
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem status_item = (switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
	}
}
