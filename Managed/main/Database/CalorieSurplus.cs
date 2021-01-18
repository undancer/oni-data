using System.IO;
using STRINGS;

namespace Database
{
	public class CalorieSurplus : ColonyAchievementRequirement
	{
		private double surplusAmount;

		public CalorieSurplus(float surplusAmount)
		{
			this.surplusAmount = surplusAmount;
		}

		public override bool Success()
		{
			return (double)(RationTracker.Get().CountRations(null) / 1000f) >= surplusAmount;
		}

		public override bool Fail()
		{
			return !Success();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(surplusAmount);
		}

		public override void Deserialize(IReader reader)
		{
			surplusAmount = reader.ReadDouble();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CALORIE_SURPLUS, GameUtil.GetFormattedCalories(complete ? ((float)surplusAmount) : RationTracker.Get().CountRations(null)), GameUtil.GetFormattedCalories((float)surplusAmount));
		}
	}
}
