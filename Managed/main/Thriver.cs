[SkipSaveFileSerialization]
public class Thriver : StateMachineComponent<Thriver.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Thriver, object>.GameInstance
	{
		public StatesInstance(Thriver master)
			: base(master)
		{
		}

		public bool IsStressed()
		{
			return base.master.GetSMI<StressMonitor.Instance>()?.IsStressed() ?? false;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Thriver>
	{
		public State idle;

		public State stressed;

		public State toostressed;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.EventTransition(GameHashes.NotStressed, idle).EventTransition(GameHashes.Stressed, stressed).EventTransition(GameHashes.StressedHadEnough, stressed)
				.Enter(delegate(StatesInstance smi)
				{
					StressMonitor.Instance sMI = smi.master.GetSMI<StressMonitor.Instance>();
					if (sMI != null && sMI.IsStressed())
					{
						smi.GoTo(stressed);
					}
				});
			idle.DoNothing();
			stressed.ToggleEffect("Thriver");
			toostressed.DoNothing();
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
