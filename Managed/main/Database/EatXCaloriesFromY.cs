using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class EatXCaloriesFromY : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private int numCalories;

		private List<string> fromFoodType = new List<string>();

		public EatXCaloriesFromY(int numCalories, List<string> fromFoodType)
		{
			this.numCalories = numCalories;
			this.fromFoodType = fromFoodType;
		}

		public override bool Success()
		{
			return RationTracker.Get().GetCaloiresConsumedByFood(fromFoodType) / 1000f > (float)numCalories;
		}

		public void Deserialize(IReader reader)
		{
			numCalories = reader.ReadInt32();
			int num = reader.ReadInt32();
			fromFoodType = new List<string>(num);
			for (int i = 0; i < num; i++)
			{
				string item = reader.ReadKleiString();
				fromFoodType.Add(item);
			}
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CALORIES_FROM_MEAT, GameUtil.GetFormattedCalories(complete ? ((float)numCalories * 1000f) : RationTracker.Get().GetCaloiresConsumedByFood(fromFoodType)), GameUtil.GetFormattedCalories((float)numCalories * 1000f));
		}
	}
}
