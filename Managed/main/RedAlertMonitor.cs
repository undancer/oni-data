public class RedAlertMonitor : GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void EnableRedAlert()
		{
			ChoreDriver component = GetComponent<ChoreDriver>();
			if (!(component != null))
			{
				return;
			}
			Chore currentChore = component.GetCurrentChore();
			if (currentChore == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < currentChore.GetPreconditions().Count; i++)
			{
				if (currentChore.GetPreconditions()[i].id == ChorePreconditions.instance.IsNotRedAlert.id)
				{
					flag = true;
				}
			}
			if (flag)
			{
				component.StopChore();
			}
		}
	}

	public State off;

	public State on;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		base.serializable = SerializeType.Both_DEPRECATED;
		off.EventTransition(GameHashes.EnteredRedAlert, (Instance smi) => Game.Instance, on, (Instance smi) => smi.master.gameObject.GetMyWorld().AlertManager.IsRedAlert());
		on.EventTransition(GameHashes.ExitedRedAlert, (Instance smi) => Game.Instance, off, (Instance smi) => !smi.master.gameObject.GetMyWorld().AlertManager.IsRedAlert()).Enter("EnableRedAlert", delegate(Instance smi)
		{
			smi.EnableRedAlert();
		}).ToggleEffect("RedAlert")
			.ToggleExpression(Db.Get().Expressions.RedAlert);
	}
}
