public class IncubatorMonitor : GameStateMachine<IncubatorMonitor, IncubatorMonitor.Instance, IStateMachineTarget, IncubatorMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	public State not;

	public State in_incubator;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = not;
		not.EventTransition(GameHashes.OnStore, in_incubator, InIncubator);
		in_incubator.ToggleTag(GameTags.Creatures.InIncubator).EventTransition(GameHashes.OnStore, not, GameStateMachine<IncubatorMonitor, Instance, IStateMachineTarget, Def>.Not(InIncubator));
	}

	public static bool InIncubator(Instance smi)
	{
		if ((bool)smi.gameObject.transform.parent)
		{
			EggIncubator component = smi.gameObject.transform.parent.GetComponent<EggIncubator>();
			return component != null;
		}
		return false;
	}
}
