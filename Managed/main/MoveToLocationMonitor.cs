using STRINGS;

public class MoveToLocationMonitor : GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public int targetCell;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			master.Subscribe(493375141, OnRefreshUserMenu);
		}

		private void OnRefreshUserMenu(object data)
		{
			Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_control", UI.USERMENUACTIONS.MOVETOLOCATION.NAME, OnClickMoveToLocation, global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.MOVETOLOCATION.TOOLTIP), 0.2f);
		}

		private void OnClickMoveToLocation()
		{
			MoveToLocationTool.Instance.Activate(GetComponent<Navigator>());
		}

		public void MoveToLocation(int cell)
		{
			targetCell = cell;
			base.smi.GoTo(base.smi.sm.satisfied);
			base.smi.GoTo(base.smi.sm.moving);
		}

		public override void StopSM(string reason)
		{
			base.master.Unsubscribe(493375141, OnRefreshUserMenu);
			base.StopSM(reason);
		}
	}

	public State satisfied;

	public State moving;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		satisfied.DoNothing();
		moving.ToggleChore((Instance smi) => new MoveChore(smi.master, Db.Get().ChoreTypes.MoveTo, (MoveChore.StatesInstance smii) => smi.targetCell), satisfied);
	}
}
