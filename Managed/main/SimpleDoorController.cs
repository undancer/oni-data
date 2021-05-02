public class SimpleDoorController : GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>
{
	public class Def : BaseDef
	{
	}

	public class ActiveStates : State
	{
		public State closed;

		public State opening;

		public State open;

		public State closedelay;

		public State closing;
	}

	public class StatesInstance : GameInstance, INavDoor
	{
		public bool isSpawned => base.master.gameObject.GetComponent<KMonoBehaviour>().isSpawned;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public string GetDefaultAnim()
		{
			KBatchedAnimController component = base.master.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				return component.initialAnim;
			}
			return "idle_loop";
		}

		public void Register()
		{
			int i = Grid.PosToCell(base.gameObject.transform.GetPosition());
			Grid.HasDoor[i] = true;
		}

		public void Unregister()
		{
			int i = Grid.PosToCell(base.gameObject.transform.GetPosition());
			Grid.HasDoor[i] = false;
		}

		public void Close()
		{
			base.sm.numOpens.Delta(-1, base.smi);
		}

		public bool IsOpen()
		{
			return IsInsideState(base.sm.active.open) || IsInsideState(base.sm.active.closedelay);
		}

		public void Open()
		{
			base.sm.numOpens.Delta(1, base.smi);
		}
	}

	public State inactive;

	public ActiveStates active;

	public IntParameter numOpens;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inactive;
		inactive.TagTransition(GameTags.RocketOnGround, active);
		active.DefaultState(active.closed).TagTransition(GameTags.RocketOnGround, inactive, on_remove: true).Enter(delegate(StatesInstance smi)
		{
			smi.Register();
		})
			.Exit(delegate(StatesInstance smi)
			{
				smi.Unregister();
			});
		active.closed.PlayAnim((StatesInstance smi) => smi.GetDefaultAnim(), KAnim.PlayMode.Loop).ParamTransition(numOpens, active.opening, (StatesInstance smi, int p) => p > 0);
		active.opening.PlayAnim("enter_pre", KAnim.PlayMode.Once).OnAnimQueueComplete(active.open);
		active.open.PlayAnim("enter_loop", KAnim.PlayMode.Loop).ParamTransition(numOpens, active.closedelay, (StatesInstance smi, int p) => p == 0);
		active.closedelay.ParamTransition(numOpens, active.open, (StatesInstance smi, int p) => p > 0).ScheduleGoTo(0.5f, active.closing);
		active.closing.PlayAnim("enter_pst", KAnim.PlayMode.Once).OnAnimQueueComplete(active.closed);
	}
}
