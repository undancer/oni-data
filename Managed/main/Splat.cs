public class Splat : GameStateMachine<Splat, Splat.StatesInstance>
{
	public class Def : BaseDef
	{
	}

	public class StatesInstance : GameInstance
	{
		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, (object)def)
		{
		}
	}

	public State complete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleChore((StatesInstance smi) => new WorkChore<SplatWorkable>(Db.Get().ChoreTypes.Mop, smi.master), complete);
		complete.Enter(delegate(StatesInstance smi)
		{
			Util.KDestroyGameObject(smi.master.gameObject);
		});
	}
}
