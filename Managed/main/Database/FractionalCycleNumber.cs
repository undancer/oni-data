using STRINGS;

namespace Database
{
	public class FractionalCycleNumber : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private float fractionalCycleNumber;

		public FractionalCycleNumber(float fractionalCycleNumber)
		{
			this.fractionalCycleNumber = fractionalCycleNumber;
		}

		public override bool Success()
		{
			int num = (int)fractionalCycleNumber;
			float num2 = fractionalCycleNumber - (float)num;
			return (float)(GameClock.Instance.GetCycle() + 1) > fractionalCycleNumber || (GameClock.Instance.GetCycle() + 1 == num && GameClock.Instance.GetCurrentCycleAsPercentage() >= num2);
		}

		public void Deserialize(IReader reader)
		{
			fractionalCycleNumber = reader.ReadSingle();
		}

		public override string GetProgress(bool complete)
		{
			float num = (float)GameClock.Instance.GetCycle() + GameClock.Instance.GetCurrentCycleAsPercentage();
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.FRACTIONAL_CYCLE, complete ? fractionalCycleNumber : num, fractionalCycleNumber);
		}
	}
}
