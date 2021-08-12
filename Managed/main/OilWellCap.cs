using Klei;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/OilWellCap")]
public class OilWellCap : Workable, ISingleSliderControl, ISliderControl, IElementEmitter
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, OilWellCap, object>.GameInstance
	{
		public StatesInstance(OilWellCap master)
			: base(master)
		{
		}

		public float GetPressurePercent()
		{
			return base.sm.pressurePercent.Get(base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, OilWellCap>
	{
		public class OperationalStates : State
		{
			public State idle;

			public PreLoopPostState active;

			public State overpressure;

			public PreLoopPostState releasing_pressure;
		}

		public FloatParameter pressurePercent;

		public BoolParameter working;

		public State inoperational;

		public OperationalStates operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inoperational;
			root.ToggleRecurringChore((StatesInstance smi) => smi.master.CreateWorkChore());
			inoperational.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, operational, IsOperational);
			operational.DefaultState(operational.idle);
			operational.idle.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing).ParamTransition(pressurePercent, operational.overpressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsGTEOne)
				.ParamTransition(working, operational.releasing_pressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsTrue)
				.EventTransition(GameHashes.OperationalChanged, inoperational, GameStateMachine<States, StatesInstance, OilWellCap, object>.Not(IsOperational))
				.EventTransition(GameHashes.OnStorageChange, operational.active, IsAbleToPump);
			operational.active.DefaultState(operational.active.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing).Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(value: false);
				})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.AddGasPressure(dt);
				});
			operational.active.pre.PlayAnim("working_pre").ParamTransition(pressurePercent, operational.overpressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition(working, operational.releasing_pressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsTrue)
				.OnAnimQueueComplete(operational.active.loop);
			operational.active.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ParamTransition(pressurePercent, operational.active.pst, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition(working, operational.active.pst, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsTrue)
				.EventTransition(GameHashes.OperationalChanged, operational.active.pst, MustStopPumping)
				.EventTransition(GameHashes.OnStorageChange, operational.active.pst, MustStopPumping);
			operational.active.pst.PlayAnim("working_pst").OnAnimQueueComplete(operational.idle);
			operational.overpressure.PlayAnim("over_pressured_pre", KAnim.PlayMode.Once).QueueAnim("over_pressured_loop", loop: true).ToggleStatusItem(Db.Get().BuildingStatusItems.WellOverpressure)
				.ParamTransition(pressurePercent, operational.idle, (StatesInstance smi, float p) => p <= 0f)
				.ParamTransition(working, operational.releasing_pressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsTrue);
			operational.releasing_pressure.DefaultState(operational.releasing_pressure.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingElement, (StatesInstance smi) => smi.master);
			operational.releasing_pressure.pre.PlayAnim("steam_out_pre").OnAnimQueueComplete(operational.releasing_pressure.loop);
			operational.releasing_pressure.loop.PlayAnim("steam_out_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, operational.releasing_pressure.pst, GameStateMachine<States, StatesInstance, OilWellCap, object>.Not(IsOperational)).ParamTransition(working, operational.releasing_pressure.pst, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsFalse)
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.ReleaseGasPressure(dt);
				});
			operational.releasing_pressure.pst.PlayAnim("steam_out_pst").OnAnimQueueComplete(operational.idle);
		}

		private bool IsOperational(StatesInstance smi)
		{
			return smi.master.operational.IsOperational;
		}

		private bool IsAbleToPump(StatesInstance smi)
		{
			if (smi.master.operational.IsOperational)
			{
				return smi.GetComponent<ElementConverter>().HasEnoughMassToStartConverting();
			}
			return false;
		}

		private bool MustStopPumping(StatesInstance smi)
		{
			if (smi.master.operational.IsOperational)
			{
				return !smi.GetComponent<ElementConverter>().CanConvertAtAll();
			}
			return true;
		}
	}

	private StatesInstance smi;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Storage storage;

	public SimHashes gasElement;

	public float gasTemperature;

	public float addGasRate = 1f;

	public float maxGasPressure = 10f;

	public float releaseGasRate = 10f;

	[Serialize]
	private float depressurizePercent = 0.75f;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private MeterController pressureMeter;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<OilWellCap> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<OilWellCap>(delegate(OilWellCap component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly Chore.Precondition AllowedToDepressurize = new Chore.Precondition
	{
		id = "AllowedToDepressurize",
		description = DUPLICANTS.CHORES.PRECONDITIONS.ALLOWED_TO_DEPRESSURIZE,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return ((OilWellCap)data).NeedsDepressurizing();
		}
	};

	public SimHashes Element => gasElement;

	public float AverageEmitRate => Game.Instance.accumulators.GetAverageRate(accumulator);

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.PERCENT;

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
		return depressurizePercent * 100f;
	}

	public void SetSliderValue(float value, int index)
	{
		depressurizePercent = value / 100f;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP";
	}

	string ISliderControl.GetSliderTooltip()
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP"), depressurizePercent * 100f);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		OilWellCap component = ((GameObject)data).GetComponent<OilWellCap>();
		if (component != null)
		{
			depressurizePercent = component.depressurizePercent;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		accumulator = Game.Instance.accumulators.Add("pressuregas", this);
		showProgressBar = false;
		SetWorkTime(float.PositiveInfinity);
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_oil_cap_kanim") };
		workingStatusItem = Db.Get().BuildingStatusItems.ReleasingPressure;
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		pressureMeter = new MeterController((KAnimControllerBase)component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new Vector3(0f, 0f, 0f), (string[])null);
		smi = new StatesInstance(this);
		smi.StartSM();
		UpdatePressurePercent();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(accumulator);
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCleanUp();
	}

	public void AddGasPressure(float dt)
	{
		storage.AddGasChunk(gasElement, addGasRate * dt, gasTemperature, 0, 0, keep_zero_mass: true);
		UpdatePressurePercent();
	}

	public void ReleaseGasPressure(float dt)
	{
		PrimaryElement primaryElement = storage.FindPrimaryElement(gasElement);
		if (primaryElement != null && primaryElement.Mass > 0f)
		{
			float num = releaseGasRate * dt;
			if (base.worker != null)
			{
				num *= GetEfficiencyMultiplier(base.worker);
			}
			num = Mathf.Min(num, primaryElement.Mass);
			SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(primaryElement, num / primaryElement.Mass);
			primaryElement.Mass -= num;
			Game.Instance.accumulators.Accumulate(accumulator, num);
			SimMessages.AddRemoveSubstance(Grid.PosToCell(this), ElementLoader.GetElementIndex(gasElement), null, num, primaryElement.Temperature, percentOfDisease.idx, percentOfDisease.count);
		}
		UpdatePressurePercent();
	}

	private void UpdatePressurePercent()
	{
		float value = storage.GetMassAvailable(gasElement) / maxGasPressure;
		value = Mathf.Clamp01(value);
		smi.sm.pressurePercent.Set(value, smi);
		pressureMeter.SetPositionPercent(value);
	}

	public bool NeedsDepressurizing()
	{
		return smi.GetPressurePercent() >= depressurizePercent;
	}

	private WorkChore<OilWellCap> CreateWorkChore()
	{
		WorkChore<OilWellCap> workChore = new WorkChore<OilWellCap>(Db.Get().ChoreTypes.Depressurize, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
		workChore.AddPrecondition(AllowedToDepressurize, this);
		return workChore;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		smi.sm.working.Set(value: true, smi);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		smi.sm.working.Set(value: false, smi);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return smi.GetPressurePercent() <= 0f;
	}

	public override bool InstantlyFinish(Worker worker)
	{
		ReleaseGasPressure(60f);
		return true;
	}
}
