using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class HighEnergyParticleSpawner : StateMachineComponent<HighEnergyParticleSpawner.StatesInstance>, IHighEnergyParticleDirection, IProgressBarSideScreen, ISingleSliderControl, ISliderControl
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, HighEnergyParticleSpawner, object>.GameInstance
	{
		public StatesInstance(HighEnergyParticleSpawner smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, HighEnergyParticleSpawner>
	{
		public class ReadyStates : State
		{
			public State create;

			public State idle;

			public State absorbing;
		}

		public BoolParameter isAbsorbingRadiation;

		public State inoperational;

		public ReadyStates ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inoperational;
			inoperational.PlayAnim("off").TagTransition(GameTags.Operational, ready).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.DoConsumeParticlesWhileDisabled(dt);
			});
			ready.TagTransition(GameTags.Operational, inoperational, on_remove: true).DefaultState(ready.idle).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.LauncherUpdate(dt);
			}, UpdateRate.SIM_EVERY_TICK);
			ready.idle.ParamTransition(isAbsorbingRadiation, ready.absorbing, GameStateMachine<States, StatesInstance, HighEnergyParticleSpawner, object>.IsTrue).PlayAnim("on");
			ready.absorbing.Enter("SetActive(true)", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			}).Exit("SetActive(false)", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: false);
			}).ParamTransition(isAbsorbingRadiation, ready.idle, GameStateMachine<States, StatesInstance, HighEnergyParticleSpawner, object>.IsFalse)
				.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		}
	}

	public static readonly HashedString PORT_ID = "HEPSpawner";

	[MyCmpReq]
	private HighEnergyParticleStorage particleStorage;

	[MyCmpGet]
	private Operational operational;

	private Guid HEPStatusHandle;

	private float recentPerSecondConsumptionRate;

	public int minSlider;

	public int maxSlider;

	private MeterController particleContainerVisual;

	[Serialize]
	private EightDirection _direction;

	public float minLaunchInterval;

	public float radiationSampleRate;

	[Serialize]
	public float particleThreshold = 50f;

	private EightDirectionController directionController;

	private float launcherTimer;

	private float radiationSampleTimer;

	private MeterController particleController;

	private bool particleVisualPlaying;

	private MeterController progressMeterController;

	[Serialize]
	public Ref<HighEnergyParticlePort> capturedByRef = new Ref<HighEnergyParticlePort>();

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleSpawner> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleSpawner>(delegate(HighEnergyParticleSpawner component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleSpawner> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleSpawner>(delegate(HighEnergyParticleSpawner component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private bool hasLogicWire = false;

	private bool isLogicActive = false;

	private static StatusItem infoStatusItem_Logic;

	public float PredictedPerCycleConsumptionRate => Mathf.FloorToInt(recentPerSecondConsumptionRate * 600f);

	public EightDirection Direction
	{
		get
		{
			return _direction;
		}
		set
		{
			_direction = value;
			if (directionController != null)
			{
				directionController.SetRotation(45 * EightDirectionUtil.GetDirectionIndex(_direction));
				directionController.controller.enabled = false;
				directionController.controller.enabled = true;
			}
		}
	}

	public bool AllowSpawnParticles => !hasLogicWire || (hasLogicWire && isLogicActive);

	public bool HasLogicWire => hasLogicWire;

	public bool IsLogicActive => isLogicActive;

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		HighEnergyParticleSpawner component = gameObject.GetComponent<HighEnergyParticleSpawner>();
		if (component != null)
		{
			Direction = component.Direction;
			particleThreshold = component.particleThreshold;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
		Subscribe(-801688580, OnLogicValueChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		KSelectable component = GetComponent<KSelectable>();
		if (infoStatusItem_Logic == null)
		{
			infoStatusItem_Logic = new StatusItem("HEPSpawnerLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			infoStatusItem_Logic.resolveStringCallback = ResolveInfoStatusItem;
			infoStatusItem_Logic.resolveTooltipCallback = ResolveInfoStatusItemTooltip;
		}
		component.AddStatusItem(infoStatusItem_Logic, this);
		HEPStatusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.CollectingHEP, this);
		directionController = new EightDirectionController(GetComponent<KBatchedAnimController>(), "redirector_target", "redirect", EightDirectionController.Offset.Infront);
		Direction = Direction;
		particleController = new MeterController(GetComponent<KBatchedAnimController>(), "orb_target", "orb_off", Meter.Offset.NoChange, Grid.SceneLayer.NoLayer);
		particleController.gameObject.AddOrGet<LoopingSounds>();
		progressMeterController = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.NoChange, Grid.SceneLayer.NoLayer);
	}

	public float GetProgressBarMaxValue()
	{
		return particleThreshold;
	}

	public float GetProgressBarFillPercentage()
	{
		return particleStorage.Particles / particleThreshold;
	}

	public string GetProgressBarTitleLabel()
	{
		return UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.PROGRESS_BAR_LABEL;
	}

	public string GetProgressBarLabel()
	{
		return Mathf.FloorToInt(particleStorage.Particles) + "/" + Mathf.FloorToInt(particleThreshold);
	}

	public string GetProgressBarTooltip()
	{
		return UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.PROGRESS_BAR_TOOLTIP;
	}

	private void OnSimConsumeRadiationCallback(Sim.ConsumedRadiationCallback radiationCBInfo, object data)
	{
	}

	public void DoConsumeParticlesWhileDisabled(float dt)
	{
		particleStorage.ConsumeAndGet(dt * 1f);
	}

	public void LauncherUpdate(float dt)
	{
		radiationSampleTimer += dt;
		if (radiationSampleTimer >= radiationSampleRate)
		{
			radiationSampleTimer -= radiationSampleRate;
			int i = Grid.PosToCell(this);
			float num = Grid.Radiation[i];
			if (num != 0f)
			{
				base.smi.sm.isAbsorbingRadiation.Set(value: true, base.smi);
				recentPerSecondConsumptionRate = num / 600f;
				particleStorage.Store(recentPerSecondConsumptionRate * radiationSampleRate);
			}
			else
			{
				base.smi.sm.isAbsorbingRadiation.Set(value: false, base.smi);
			}
		}
		progressMeterController.SetPositionPercent(GetProgressBarFillPercentage());
		if (!particleVisualPlaying && particleStorage.Particles > particleThreshold / 2f)
		{
			particleController.meterController.Play("orb_pre");
			particleController.meterController.Queue("orb_idle", KAnim.PlayMode.Loop);
			particleVisualPlaying = true;
		}
		launcherTimer += dt;
		if (!(launcherTimer < minLaunchInterval) && AllowSpawnParticles && particleStorage.Particles >= particleThreshold)
		{
			launcherTimer = 0f;
			int highEnergyParticleOutputCell = GetComponent<Building>().GetHighEnergyParticleOutputCell();
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
			gameObject.SetActive(value: true);
			if (gameObject != null)
			{
				HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
				component.payload = particleStorage.ConsumeAndGet(particleThreshold);
				component.SetDirection(Direction);
				directionController.PlayAnim("redirect_send");
				directionController.controller.Queue("redirect");
				particleController.meterController.Play("orb_send");
				particleController.meterController.Queue("orb_off");
				particleVisualPlaying = false;
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private LogicCircuitNetwork GetNetwork()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		int portCell = component.GetPortCell(PORT_ID);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		return logicCircuitManager.GetNetworkForCell(portCell);
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			isLogicActive = logicValueChanged.newValue > 0;
			hasLogicWire = GetNetwork() != null;
		}
	}

	private static string ResolveInfoStatusItem(string format_str, object data)
	{
		HighEnergyParticleSpawner highEnergyParticleSpawner = (HighEnergyParticleSpawner)data;
		if (!highEnergyParticleSpawner.HasLogicWire)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.NORMAL;
		}
		if (highEnergyParticleSpawner.IsLogicActive)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.LOGIC_CONTROLLED_ACTIVE;
		}
		return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.LOGIC_CONTROLLED_STANDBY;
	}

	private static string ResolveInfoStatusItemTooltip(string format_str, object data)
	{
		HighEnergyParticleSpawner highEnergyParticleSpawner = (HighEnergyParticleSpawner)data;
		if (!highEnergyParticleSpawner.HasLogicWire)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.TOOLTIPS.NORMAL;
		}
		if (highEnergyParticleSpawner.IsLogicActive)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.TOOLTIPS.LOGIC_CONTROLLED_ACTIVE;
		}
		return BUILDING.STATUSITEMS.HIGHENERGYPARTICLESPAWNER.TOOLTIPS.LOGIC_CONTROLLED_STANDBY;
	}

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return minSlider;
	}

	public float GetSliderMax(int index)
	{
		return maxSlider;
	}

	public float GetSliderValue(int index)
	{
		return particleThreshold;
	}

	public void SetSliderValue(float value, int index)
	{
		particleThreshold = value;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.TOOLTIP";
	}

	string ISliderControl.GetSliderTooltip()
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.HIGHENERGYPARTICLESPAWNERSIDESCREEN.TOOLTIP"), particleThreshold);
	}
}
