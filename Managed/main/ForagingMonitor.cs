using UnityEngine;

public class ForagingMonitor : GameStateMachine<ForagingMonitor, ForagingMonitor.Instance, IStateMachineTarget, ForagingMonitor.Def>
{
	public class Def : BaseDef
	{
		public float searchMinInterval = 150f;

		public float searchMaxInterval = 240f;
	}

	public new class Instance : GameInstance
	{
		public float nextSearchTime;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			RefreshSearchTime();
		}

		public void RefreshSearchTime()
		{
			nextSearchTime = Time.time + Mathf.Lerp(base.def.searchMinInterval, base.def.searchMaxInterval, Random.value);
		}
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = root;
		root.ToggleBehaviour(GameTags.Creatures.WantsToForage, ShouldForage, delegate(Instance smi)
		{
			smi.RefreshSearchTime();
		});
	}

	public static bool ShouldForage(Instance smi)
	{
		return Time.time >= smi.nextSearchTime;
	}
}
