using STRINGS;
using UnityEngine;

namespace Database
{
	public class BeforeCycleNumber : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private int cycleNumber;

		public BeforeCycleNumber(int cycleNumber = 100)
		{
			this.cycleNumber = cycleNumber;
		}

		public override bool Success()
		{
			return GameClock.Instance.GetCycle() + 1 <= cycleNumber;
		}

		public override bool Fail()
		{
			return !Success();
		}

		public void Deserialize(IReader reader)
		{
			cycleNumber = reader.ReadInt32();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.REMAINING_CYCLES, Mathf.Max(cycleNumber - GameClock.Instance.GetCycle(), 0), cycleNumber);
		}
	}
}
