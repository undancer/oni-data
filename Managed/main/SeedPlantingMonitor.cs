using UnityEngine;

public class SeedPlantingMonitor : GameStateMachine<SeedPlantingMonitor, SeedPlantingMonitor.Instance, IStateMachineTarget, SeedPlantingMonitor.Def>
{
	public class Def : BaseDef
	{
		public float searchMinInterval = 60f;

		public float searchMaxInterval = 300f;
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
		root.ToggleBehaviour(GameTags.Creatures.WantsToPlantSeed, ShouldSearchForSeeds, delegate(Instance smi)
		{
			smi.RefreshSearchTime();
		});
	}

	public static bool ShouldSearchForSeeds(Instance smi)
	{
		return Time.time >= smi.nextSearchTime;
	}
}
