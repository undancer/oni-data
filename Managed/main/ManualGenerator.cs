using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/ManualGenerator")]
public class ManualGenerator : Workable, ISingleSliderControl, ISliderControl
{
	public class GeneratePowerSM : GameStateMachine<GeneratePowerSM, GeneratePowerSM.Instance>
	{
		public class WorkingStates : State
		{
			public State pre;

			public State loop;

			public State pst;
		}

		public new class Instance : GameInstance
		{
			public Instance(IStateMachineTarget master)
				: base(master)
			{
			}
		}

		public State off;

		public State on;

		public WorkingStates working;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			base.serializable = SerializeType.Both_DEPRECATED;
			off.EventTransition(GameHashes.OperationalChanged, on, (Instance smi) => smi.master.GetComponent<Operational>().IsOperational).PlayAnim("off");
			on.EventTransition(GameHashes.OperationalChanged, off, (Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, working.pre, (Instance smi) => smi.master.GetComponent<Operational>().IsActive).PlayAnim("on");
			working.DefaultState(working.pre);
			working.pre.PlayAnim("working_pre").OnAnimQueueComplete(working.loop);
			working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.ActiveChanged, off, (Instance smi) => masterTarget.Get(smi) != null && !smi.master.GetComponent<Operational>().IsActive);
		}
	}

	[Serialize]
	[SerializeField]
	private float batteryRefillPercent = 0.5f;

	private const float batteryStopRunningPercent = 1f;

	[MyCmpReq]
	private Generator generator;

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private BuildingEnabledButton buildingEnabledButton;

	private Chore chore;

	private int powerCell;

	private GeneratePowerSM.Instance smi;

	private static readonly KAnimHashedString[] symbol_names = new KAnimHashedString[6] { "meter", "meter_target", "meter_fill", "meter_frame", "meter_light", "meter_tubing" };

	private static readonly EventSystem.IntraObjectHandler<ManualGenerator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ManualGenerator>(delegate(ManualGenerator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ManualGenerator> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ManualGenerator>(delegate(ManualGenerator component, object data)
	{
		component.OnActiveChanged(data);
	});

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.PERCENT;

	public bool IsPowered => operational.IsActive;

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 100f;
	}

	public float GetSliderValue(int index)
	{
		return batteryRefillPercent * 100f;
	}

	public void SetSliderValue(float value, int index)
	{
		batteryRefillPercent = value / 100f;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TOOLTIP";
	}

	string ISliderControl.GetSliderTooltip()
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TOOLTIP"), batteryRefillPercent * 100f);
	}

	private ManualGenerator()
	{
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-592767678, OnOperationalChangedDelegate);
		Subscribe(824508782, OnActiveChangedDelegate);
		workerStatusItem = Db.Get().DuplicantStatusItems.GeneratingPower;
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		EnergyGenerator.EnsureStatusItemAvailable();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetWorkTime(float.PositiveInfinity);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		KAnimHashedString[] array = symbol_names;
		foreach (KAnimHashedString symbol in array)
		{
			component.SetSymbolVisiblity(symbol, is_visible: false);
		}
		Building component2 = GetComponent<Building>();
		powerCell = component2.GetPowerOutputCell();
		OnActiveChanged(null);
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_generatormanual_kanim") };
		smi = new GeneratePowerSM.Instance(this);
		smi.StartSM();
		Game.Instance.energySim.AddManualGenerator(this);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.energySim.RemoveManualGenerator(this);
		smi.StopSM("cleanup");
		base.OnCleanUp();
	}

	protected void OnActiveChanged(object is_active)
	{
		if (operational.IsActive)
		{
			GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.ManualGeneratorChargingUp);
		}
	}

	public void EnergySim200ms(float dt)
	{
		KSelectable component = GetComponent<KSelectable>();
		if (operational.IsActive)
		{
			generator.GenerateJoules(generator.WattageRating * dt);
			component.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, generator);
			return;
		}
		generator.ResetJoules();
		component.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.GeneratorOffline);
		if (!operational.IsOperational)
		{
			return;
		}
		CircuitManager circuitManager = Game.Instance.circuitManager;
		if (circuitManager == null)
		{
			return;
		}
		ushort circuitID = circuitManager.GetCircuitID(generator);
		bool flag = circuitManager.HasBatteries(circuitID);
		bool flag2 = false;
		if (!flag && circuitManager.HasConsumers(circuitID))
		{
			flag2 = true;
		}
		else if (flag)
		{
			if (batteryRefillPercent <= 0f && circuitManager.GetMinBatteryPercentFullOnCircuit(circuitID) <= 0f)
			{
				flag2 = true;
			}
			else if (circuitManager.GetMinBatteryPercentFullOnCircuit(circuitID) < batteryRefillPercent)
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			if (chore == null && smi.GetCurrentState() == smi.sm.on)
			{
				chore = new WorkChore<ManualGenerator>(Db.Get().ChoreTypes.GeneratePower, this);
			}
		}
		else if (chore != null)
		{
			chore.Cancel("No refill needed");
			chore = null;
		}
		component.ToggleStatusItem(EnergyGenerator.BatteriesSufficientlyFull, !flag2);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		operational.SetActive(value: true);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		CircuitManager circuitManager = Game.Instance.circuitManager;
		bool flag = false;
		if (circuitManager != null)
		{
			ushort circuitID = circuitManager.GetCircuitID(generator);
			bool flag2 = circuitManager.HasBatteries(circuitID);
			flag = (flag2 && circuitManager.GetMinBatteryPercentFullOnCircuit(circuitID) < 1f) || (!flag2 && circuitManager.HasConsumers(circuitID));
		}
		AttributeLevels component = worker.GetComponent<AttributeLevels>();
		if (component != null)
		{
			component.AddExperience(Db.Get().Attributes.Athletics.Id, dt, DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE);
		}
		return !flag;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		operational.SetActive(value: false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		operational.SetActive(value: false);
		if (chore != null)
		{
			chore.Cancel("complete");
			chore = null;
		}
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}

	private void OnOperationalChanged(object data)
	{
		if (!buildingEnabledButton.IsEnabled)
		{
			generator.ResetJoules();
		}
	}
}
