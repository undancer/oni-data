using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RocketEngine : StateMachineComponent<RocketEngine.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RocketEngine, object>.GameInstance
	{
		public StatesInstance(RocketEngine smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RocketEngine>
	{
		public State idle;

		public State burning;

		public State burnComplete;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.IgniteEngine, burning);
			burning.EventTransition(GameHashes.RocketLanded, burnComplete).PlayAnim("launch_pre").QueueAnim("launch_loop", loop: true)
				.Update(delegate(StatesInstance smi, float dt)
				{
					Vector3 pos = smi.master.gameObject.transform.GetPosition() + smi.master.GetComponent<KBatchedAnimController>().Offset;
					int num = Grid.PosToCell(pos);
					if (Grid.IsValidCell(num))
					{
						SimMessages.EmitMass(num, (byte)ElementLoader.GetElementIndex(smi.master.exhaustElement), dt * smi.master.exhaustEmitRate, smi.master.exhaustTemperature, 0, 0);
					}
					int num2 = 10;
					for (int i = 1; i < num2; i++)
					{
						int num3 = Grid.OffsetCell(num, -1, -i);
						int num4 = Grid.OffsetCell(num, 0, -i);
						int num5 = Grid.OffsetCell(num, 1, -i);
						if (Grid.IsValidCell(num3))
						{
							SimMessages.ModifyEnergy(num3, smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
						}
						if (Grid.IsValidCell(num4))
						{
							SimMessages.ModifyEnergy(num4, smi.master.exhaustTemperature / (float)i, 3200f, SimMessages.EnergySourceID.Burner);
						}
						if (Grid.IsValidCell(num5))
						{
							SimMessages.ModifyEnergy(num5, smi.master.exhaustTemperature / (float)(i + 1), 3200f, SimMessages.EnergySourceID.Burner);
						}
					}
				})
				.Exit(delegate
				{
				});
			burnComplete.PlayAnim("grounded", KAnim.PlayMode.Loop).EventTransition(GameHashes.IgniteEngine, burning);
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

	public bool mainEngine = true;

	public Light2D flameLight;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (mainEngine)
		{
			GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new RequireAttachedComponent(base.gameObject.GetComponent<AttachableBuilding>(), typeof(FuelTank), UI.STARMAP.COMPONENT.FUEL_TANK));
			GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, new ConditionModuleCount(this));
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
