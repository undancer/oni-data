using UnityEngine;

public class BeeForagingMonitor : GameStateMachine<BeeForagingMonitor, BeeForagingMonitor.Instance, IStateMachineTarget, BeeForagingMonitor.Def>
{
	public class Def : BaseDef
	{
		public float searchMinInterval = 0.25f;

		public float searchMaxInterval = 0.3f;
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
			nextSearchTime = GameClock.Instance.GetTimeInCycles() + Mathf.Lerp(base.def.searchMinInterval, base.def.searchMaxInterval, Random.value);
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
		bool flag = GameClock.Instance.GetTimeInCycles() >= smi.nextSearchTime;
		KPrefabID kPrefabID = smi.master.GetComponent<Bee>().FindHiveInRoom();
		if (kPrefabID != null)
		{
			BeehiveCalorieMonitor.Instance sMI = kPrefabID.GetSMI<BeehiveCalorieMonitor.Instance>();
			if (sMI == null || !sMI.IsHungry())
			{
				flag = false;
			}
		}
		if (flag)
		{
			return kPrefabID != null;
		}
		return false;
	}
}
