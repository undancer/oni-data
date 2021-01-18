using KSerialization;
using STRINGS;

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
			burning.EventTransition(GameHashes.LandRocket, burnComplete).PlayAnim("launch_pre").QueueAnim("launch_loop", loop: true)
				.Update(delegate(StatesInstance smi, float dt)
				{
					int num = Grid.PosToCell(smi.master.gameObject.transform.GetPosition() + smi.master.GetComponent<KBatchedAnimController>().Offset);
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

	public bool mainEngine = true;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (mainEngine)
		{
			RequireAttachedComponent condition = new RequireAttachedComponent(base.gameObject.GetComponent<AttachableBuilding>(), typeof(FuelTank), UI.STARMAP.COMPONENT.FUEL_TANK);
			GetComponent<RocketModule>().AddLaunchCondition(condition);
		}
	}
}
