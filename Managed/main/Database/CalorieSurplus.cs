using STRINGS;

namespace Database
{
	public class CalorieSurplus : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private double surplusAmount;

		public CalorieSurplus(float surplusAmount)
		{
			this.surplusAmount = surplusAmount;
		}

		public override bool Success()
		{
			return (double)(ClusterManager.Instance.CountAllRations() / 1000f) >= surplusAmount;
		}

		public override bool Fail()
		{
			return !Success();
		}

		public void Deserialize(IReader reader)
		{
			surplusAmount = reader.ReadDouble();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CALORIE_SURPLUS, GameUtil.GetFormattedCalories(complete ? ((float)surplusAmount) : ClusterManager.Instance.CountAllRations()), GameUtil.GetFormattedCalories((float)surplusAmount));
		}
	}
}
