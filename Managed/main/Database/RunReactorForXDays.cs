using STRINGS;

namespace Database
{
	public class RunReactorForXDays : ColonyAchievementRequirement
	{
		private int numCycles;

		public RunReactorForXDays(int numCycles)
		{
			this.numCycles = numCycles;
		}

		public override string GetProgress(bool complete)
		{
			int num = 0;
			foreach (Reactor item in Components.NuclearReactors.Items)
			{
				if (item.numCyclesRunning > num)
				{
					num = item.numCyclesRunning;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.RUN_A_REACTOR, complete ? numCycles : num, numCycles);
		}

		public override bool Success()
		{
			foreach (Reactor item in Components.NuclearReactors.Items)
			{
				if (item.numCyclesRunning >= numCycles)
				{
					return true;
				}
			}
			return false;
		}
	}
}
