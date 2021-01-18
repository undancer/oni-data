using STRINGS;

namespace Database
{
	public class CycleNumber : VictoryColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private int cycleNumber;

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_CYCLE, cycleNumber);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_CYCLE_DESCRIPTION, cycleNumber);
		}

		public CycleNumber(int cycleNumber = 100)
		{
			this.cycleNumber = cycleNumber;
		}

		public override bool Success()
		{
			return GameClock.Instance.GetCycle() + 1 >= cycleNumber;
		}

		public void Deserialize(IReader reader)
		{
			cycleNumber = reader.ReadInt32();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CYCLE_NUMBER, complete ? cycleNumber : (GameClock.Instance.GetCycle() + 1), cycleNumber);
		}
	}
}
