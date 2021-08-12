using STRINGS;

public class MassiveHeatSink : StateMachineComponent<MassiveHeatSink.StatesInstance>
{
	public class States : GameStateMachine<States, StatesInstance, MassiveHeatSink>
	{
		public State disabled;

		public State idle;

		public State active;

		private string AwaitingFuelResolveString(string str, object obj)
		{
			ElementConverter elementConverter = ((StatesInstance)obj).master.elementConverter;
			string arg = elementConverter.consumedElements[0].tag.ProperName();
			string formattedMass = GameUtil.GetFormattedMass(elementConverter.consumedElements[0].massConsumptionRate, GameUtil.TimeSlice.PerSecond);
			str = string.Format(str, arg, formattedMass);
			return str;
		}

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			disabled.EventTransition(GameHashes.OperationalChanged, idle, (StatesInstance smi) => smi.master.operational.IsOperational);
			idle.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.master.operational.IsOperational).ToggleStatusItem(BUILDING.STATUSITEMS.AWAITINGFUEL.NAME, BUILDING.STATUSITEMS.AWAITINGFUEL.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, default(HashedString), 129022, AwaitingFuelResolveString).EventTransition(GameHashes.OnStorageChange, active, (StatesInstance smi) => smi.master.elementConverter.HasEnoughMassToStartConverting());
			active.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.master.operational.IsOperational).EventTransition(GameHashes.OnStorageChange, idle, (StatesInstance smi) => !smi.master.elementConverter.HasEnoughMassToStartConverting()).Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(value: true);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(value: false);
				});
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, MassiveHeatSink, object>.GameInstance
	{
		public StatesInstance(MassiveHeatSink master)
			: base(master)
		{
		}
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private ElementConverter elementConverter;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}
}
