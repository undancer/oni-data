using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RocketEngineCluster : StateMachineComponent<RocketEngineCluster.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RocketEngineCluster, object>.GameInstance
	{
		public Vector3 radiationEmissionBaseOffset;

		private int pad_cell;

		public StatesInstance(RocketEngineCluster smi)
			: base(smi)
		{
			if (smi.emitRadiation)
			{
				DebugUtil.Assert(smi.radiationEmitter != null, "emitRadiation enabled but no RadiationEmitter component");
				radiationEmissionBaseOffset = smi.radiationEmitter.emissionOffset;
			}
		}

		public void BeginBurn()
		{
			if (base.smi.master.emitRadiation)
			{
				base.smi.master.radiationEmitter.SetEmitting(emitting: true);
			}
			LaunchPad currentPad = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
			if (currentPad != null)
			{
				pad_cell = Grid.PosToCell(currentPad.gameObject.transform.GetPosition());
				if (base.smi.master.exhaustDiseaseIdx != byte.MaxValue)
				{
					currentPad.GetComponent<PrimaryElement>().AddDisease(base.smi.master.exhaustDiseaseIdx, base.smi.master.exhaustDiseaseCount, "rocket exhaust");
				}
			}
			else
			{
				Debug.LogWarning("RocketEngineCluster missing LaunchPad for burn.");
				pad_cell = Grid.InvalidCell;
			}
		}

		public void DoBurn(float dt)
		{
			int num = Grid.PosToCell(base.smi.master.gameObject.transform.GetPosition() + base.smi.master.animController.Offset);
			if (Grid.AreCellsInSameWorld(num, pad_cell))
			{
				SimMessages.EmitMass(num, (byte)ElementLoader.GetElementIndex(base.smi.master.exhaustElement), dt * base.smi.master.exhaustEmitRate, base.smi.master.exhaustTemperature, base.smi.master.exhaustDiseaseIdx, base.smi.master.exhaustDiseaseCount);
			}
			if (base.smi.master.emitRadiation)
			{
				Vector3 emissionOffset = base.smi.master.radiationEmitter.emissionOffset;
				base.smi.master.radiationEmitter.emissionOffset = base.smi.radiationEmissionBaseOffset + base.smi.master.animController.Offset;
				if (Grid.AreCellsInSameWorld(base.smi.master.radiationEmitter.GetEmissionCell(), pad_cell))
				{
					base.smi.master.radiationEmitter.Refresh();
				}
				else
				{
					base.smi.master.radiationEmitter.emissionOffset = emissionOffset;
					base.smi.master.radiationEmitter.SetEmitting(emitting: false);
				}
			}
			int num2 = 10;
			for (int i = 1; i < num2; i++)
			{
				int num3 = Grid.OffsetCell(num, -1, -i);
				int num4 = Grid.OffsetCell(num, 0, -i);
				int num5 = Grid.OffsetCell(num, 1, -i);
				if (Grid.AreCellsInSameWorld(num3, pad_cell))
				{
					if (base.smi.master.exhaustDiseaseIdx != byte.MaxValue)
					{
						SimMessages.ModifyDiseaseOnCell(num3, base.smi.master.exhaustDiseaseIdx, (int)((float)base.smi.master.exhaustDiseaseCount / ((float)i + 1f)));
					}
					SimMessages.ModifyEnergy(num3, base.smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
				}
				if (Grid.AreCellsInSameWorld(num4, pad_cell))
				{
					if (base.smi.master.exhaustDiseaseIdx != byte.MaxValue)
					{
						SimMessages.ModifyDiseaseOnCell(num4, base.smi.master.exhaustDiseaseIdx, (int)((float)base.smi.master.exhaustDiseaseCount / (float)i));
					}
					SimMessages.ModifyEnergy(num4, base.smi.master.exhaustTemperature / (float)i, 3200f, SimMessages.EnergySourceID.Burner);
				}
				if (Grid.AreCellsInSameWorld(num5, pad_cell))
				{
					if (base.smi.master.exhaustDiseaseIdx != byte.MaxValue)
					{
						SimMessages.ModifyDiseaseOnCell(num5, base.smi.master.exhaustDiseaseIdx, (int)((float)base.smi.master.exhaustDiseaseCount / ((float)i + 1f)));
					}
					SimMessages.ModifyEnergy(num5, base.smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
				}
			}
		}

		public void EndBurn()
		{
			if (base.smi.master.emitRadiation)
			{
				base.smi.master.radiationEmitter.emissionOffset = base.smi.radiationEmissionBaseOffset;
				base.smi.master.radiationEmitter.SetEmitting(emitting: false);
			}
			pad_cell = Grid.InvalidCell;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RocketEngineCluster>
	{
		public class InitializingStates : State
		{
			public State load;

			public State decide;
		}

		public class IdleStates : State
		{
			public State grounded;

			public State ready;
		}

		public InitializingStates initializing;

		public IdleStates idle;

		public State burning_pre;

		public State burning;

		public State burnComplete;

		public State space;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = initializing.load;
			initializing.load.ScheduleGoTo(0f, initializing.decide);
			initializing.decide.Transition(space, IsRocketInSpace).Transition(burning, IsRocketAirborne).Transition(idle, IsRocketGrounded);
			idle.DefaultState(idle.grounded).EventTransition(GameHashes.RocketLaunched, burning_pre);
			idle.grounded.EventTransition(GameHashes.LaunchConditionChanged, idle.ready, IsReadyToLaunch).QueueAnim("grounded", loop: true);
			idle.ready.EventTransition(GameHashes.LaunchConditionChanged, idle.grounded, GameStateMachine<States, StatesInstance, RocketEngineCluster, object>.Not(IsReadyToLaunch)).PlayAnim("pre_ready_to_launch", KAnim.PlayMode.Once).QueueAnim("ready_to_launch", loop: true)
				.Exit(delegate(StatesInstance smi)
				{
					KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
					if (component != null)
					{
						component.Play("pst_ready_to_launch");
					}
				});
			burning_pre.PlayAnim("launch_pre").OnAnimQueueComplete(burning);
			burning.EventTransition(GameHashes.RocketLanded, burnComplete).PlayAnim("launch_loop", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.BeginBurn();
			})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.DoBurn(dt);
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.EndBurn();
				})
				.TagTransition(GameTags.RocketInSpace, space);
			space.EventTransition(GameHashes.DoReturnRocket, burning);
			burnComplete.PlayAnim("launch_pst", KAnim.PlayMode.Loop).GoTo(idle);
		}

		private bool IsReadyToLaunch(StatesInstance smi)
		{
			return smi.GetComponent<RocketModuleCluster>().CraftInterface.CheckPreppedForLaunch();
		}

		public bool IsRocketAirborne(StatesInstance smi)
		{
			if (smi.master.HasTag(GameTags.RocketNotOnGround))
			{
				return !smi.master.HasTag(GameTags.RocketInSpace);
			}
			return false;
		}

		public bool IsRocketGrounded(StatesInstance smi)
		{
			return smi.master.HasTag(GameTags.RocketOnGround);
		}

		public bool IsRocketInSpace(StatesInstance smi)
		{
			return smi.master.HasTag(GameTags.RocketInSpace);
		}
	}

	public float exhaustEmitRate = 50f;

	public float exhaustTemperature = 1500f;

	public SpawnFXHashes explosionEffectHash;

	public SimHashes exhaustElement = SimHashes.CarbonDioxide;

	public Tag fuelTag;

	public float efficiency = 1f;

	public bool requireOxidizer = true;

	public int maxModules = 32;

	public int maxHeight;

	public bool mainEngine = true;

	public byte exhaustDiseaseIdx = byte.MaxValue;

	public int exhaustDiseaseCount;

	public bool emitRadiation;

	[MyCmpGet]
	private RadiationEmitter radiationEmitter;

	[MyCmpGet]
	private Generator powerGenerator;

	[MyCmpReq]
	private KBatchedAnimController animController;

	public Light2D flameLight;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (mainEngine)
		{
			GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new RequireAttachedComponent(base.gameObject.GetComponent<AttachableBuilding>(), typeof(IFuelTank), UI.STARMAP.COMPONENT.FUEL_TANK));
			if (requireOxidizer)
			{
				GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new RequireAttachedComponent(base.gameObject.GetComponent<AttachableBuilding>(), typeof(OxidizerTank), UI.STARMAP.COMPONENT.OXIDIZER_TANK));
			}
			GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionRocketHeight(this));
		}
	}

	private void ConfigureFlameLight()
	{
		flameLight = base.gameObject.AddOrGet<Light2D>();
		flameLight.Color = Color.white;
		flameLight.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
		flameLight.Range = 10f;
		flameLight.Angle = 0f;
		flameLight.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
		flameLight.Offset = LIGHT2D.LIGHTBUG_OFFSET;
		flameLight.shape = LightShape.Circle;
		flameLight.drawOverlay = true;
		flameLight.Lux = 80000;
		flameLight.emitter.RemoveFromGrid();
		base.gameObject.AddOrGet<LightSymbolTracker>().targetSymbol = GetComponent<KBatchedAnimController>().CurrentAnim.rootSymbol;
		flameLight.enabled = false;
	}

	private void UpdateFlameLight(int cell)
	{
		base.smi.master.flameLight.RefreshShapeAndPosition();
		if (Grid.IsValidCell(cell))
		{
			if (!base.smi.master.flameLight.enabled && base.smi.timeinstate > 3f)
			{
				base.smi.master.flameLight.enabled = true;
			}
		}
		else
		{
			base.smi.master.flameLight.enabled = false;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}
}
