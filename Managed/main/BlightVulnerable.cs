[SkipSaveFileSerialization]
public class BlightVulnerable : StateMachineComponent<BlightVulnerable.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BlightVulnerable, object>.GameInstance
	{
		public StatesInstance(BlightVulnerable master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BlightVulnerable>
	{
		public BoolParameter isBlighted;

		public State comfortable;

		public State blighted;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = comfortable;
			base.serializable = SerializeType.Both_DEPRECATED;
			comfortable.ParamTransition(isBlighted, blighted, GameStateMachine<States, StatesInstance, BlightVulnerable, object>.IsTrue);
			blighted.TriggerOnEnter(GameHashes.BlightChanged, (StatesInstance smi) => true).Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<SeedProducer>().seedInfo.seedId = RotPileConfig.ID;
			}).ToggleTag(GameTags.Blighted)
				.Exit(delegate(StatesInstance smi)
				{
					GameplayEventManager.Instance.Trigger(-1425542080, smi.gameObject);
				});
		}
	}

	private SchedulerHandle handle;

	public bool prefersDarkness;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void MakeBlighted()
	{
		Debug.Log("Blighting plant", this);
		base.smi.sm.isBlighted.Set(value: true, base.smi);
	}
}
