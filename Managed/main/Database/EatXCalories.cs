using System.IO;
using STRINGS;

namespace Database
{
	public class EatXCalories : ColonyAchievementRequirement
	{
		private int numCalories;

		public EatXCalories(int numCalories)
		{
			this.numCalories = numCalories;
		}

		public override bool Success()
		{
			return RationTracker.Get().GetCaloriesConsumed() / 1000f > (float)numCalories;
		}

		public override void Deserialize(IReader reader)
		{
			numCalories = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(numCalories);
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CONSUME_CALORIES, GameUtil.GetFormattedCalories(complete ? ((float)numCalories * 1000f) : RationTracker.Get().GetCaloriesConsumed()), GameUtil.GetFormattedCalories((float)numCalories * 1000f));
		}
	}
}
