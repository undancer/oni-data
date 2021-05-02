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

		public StatesInstance(RocketEngineCluster smi)
			: base(smi)
		{
			if (smi.emitRadiation)
			{
				DebugUtil.Assert(smi.radiationEmitter != null, "emitRadiation enabled but no RadiationEmitter component");
				radiationEmissionBaseOffset = smi.radiationEmitter.emissionOffset;
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RocketEngineCluster>
	{
		public class IdleStates : State
		{
			public State grounded;

			public State ready;
		}

		public IdleStates idle;

		public State burning;

		public State burnComplete;

		public State space;

		private int pad_cell;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.DefaultState(idle.grounded).EventTransition(GameHashes.IgniteEngine, burning);
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
			burning.EventTransition(GameHashes.RocketLanded, burnComplete).PlayAnim("launch_pre").QueueAnim("launch_loop", loop: true)
				.Enter(delegate(StatesInstance smi)
				{
					if (smi.master.emitRadiation)
					{
						smi.master.radiationEmitter.SetEmitting(emitting: true);
					}
					LaunchPad currentPad = smi.master.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
					if (currentPad != null)
					{
						pad_cell = Grid.PosToCell(currentPad.gameObject.transform.GetPosition());
						if (smi.master.exhaustDiseaseIdx != byte.MaxValue)
						{
							currentPad.GetComponent<PrimaryElement>().AddDisease(smi.master.exhaustDiseaseIdx, smi.master.exhaustDiseaseCount, "rocket exhaust");
						}
					}
					else
					{
						Debug.LogWarning("RocketEngineCluster missing LaunchPad for burn.");
						pad_cell = Grid.InvalidCell;
					}
				})
				.Update(delegate(StatesInstance smi, float dt)
				{
					Vector3 pos = smi.master.gameObject.transform.GetPosition() + smi.master.animController.Offset;
					int num = Grid.PosToCell(pos);
					if (Grid.AreCellsInSameWorld(num, pad_cell))
					{
						SimMessages.EmitMass(num, (byte)ElementLoader.GetElementIndex(smi.master.exhaustElement), dt * smi.master.exhaustEmitRate, smi.master.exhaustTemperature, smi.master.exhaustDiseaseIdx, smi.master.exhaustDiseaseCount);
					}
					if (smi.master.emitRadiation)
					{
						Vector3 emissionOffset = smi.master.radiationEmitter.emissionOffset;
						smi.master.radiationEmitter.emissionOffset = smi.radiationEmissionBaseOffset + smi.master.animController.Offset;
						if (Grid.AreCellsInSameWorld(smi.master.radiationEmitter.GetEmissionCell(), pad_cell))
						{
							smi.master.radiationEmitter.Refresh();
						}
						else
						{
							smi.master.radiationEmitter.emissionOffset = emissionOffset;
							smi.master.radiationEmitter.SetEmitting(emitting: false);
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
							if (smi.master.exhaustDiseaseIdx != byte.MaxValue)
							{
								SimMessages.ModifyDiseaseOnCell(num3, smi.master.exhaustDiseaseIdx, (int)((float)smi.master.exhaustDiseaseCount / ((float)i + 1f)));
							}
							SimMessages.ModifyEnergy(num3, smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
						}
						if (Grid.AreCellsInSameWorld(num4, pad_cell))
						{
							if (smi.master.exhaustDiseaseIdx != byte.MaxValue)
							{
								SimMessages.ModifyDiseaseOnCell(num4, smi.master.exhaustDiseaseIdx, (int)((float)smi.master.exhaustDiseaseCount / (float)i));
							}
							SimMessages.ModifyEnergy(num4, smi.master.exhaustTemperature / (float)i, 3200f, SimMessages.EnergySourceID.Burner);
						}
						if (Grid.AreCellsInSameWorld(num5, pad_cell))
						{
							if (smi.master.exhaustDiseaseIdx != byte.MaxValue)
							{
								SimMessages.ModifyDiseaseOnCell(num5, smi.master.exhaustDiseaseIdx, (int)((float)smi.master.exhaustDiseaseCount / ((float)i + 1f)));
							}
							SimMessages.ModifyEnergy(num5, smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
						}
					}
				})
				.Exit(delegate(StatesInstance smi)
				{
					if (smi.master.emitRadiation)
					{
						smi.master.radiationEmitter.emissionOffset = smi.radiationEmissionBaseOffset;
						smi.master.radiationEmitter.SetEmitting(emitting: false);
					}
					pad_cell = Grid.InvalidCell;
				})
				.TagTransition(GameTags.RocketInSpace, space);
			space.EventTransition(GameHashes.DoReturnRocket, burning);
			burnComplete.PlayAnim("grounded", KAnim.PlayMode.Loop).GoTo(idle);
		}

		private bool IsReadyToLaunch(StatesInstance smi)
		{
			RocketModuleCluster component = smi.GetComponent<RocketModuleCluster>();
			return component.CraftInterface.CheckPreppedForLaunch();
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

	public int maxHeight = 0;

	public bool mainEngine = true;

	public byte exhaustDiseaseIdx = byte.MaxValue;

	public int exhaustDiseaseCount = 0;

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
