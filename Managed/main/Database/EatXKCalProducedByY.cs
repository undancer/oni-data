using System.Collections.Generic;
using System.Linq;
using STRINGS;

namespace Database
{
	public class EatXKCalProducedByY : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private int numCalories;

		private List<Tag> foodProducers;

		public EatXKCalProducedByY(int numCalories, List<Tag> foodProducers)
		{
			this.numCalories = numCalories;
			this.foodProducers = foodProducers;
		}

		public override bool Success()
		{
			List<string> list = new List<string>();
			foreach (ComplexRecipe recipe in ComplexRecipeManager.Get().recipes)
			{
				foreach (Tag foodProducer in foodProducers)
				{
					foreach (Tag fabricator in recipe.fabricators)
					{
						if (fabricator == foodProducer)
						{
							list.Add(recipe.FirstResult.ToString());
						}
					}
				}
			}
			return RationTracker.Get().GetCaloiresConsumedByFood(list.Distinct().ToList()) / 1000f > (float)numCalories;
		}

		public void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			foodProducers = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadKleiString();
				foodProducers.Add(new Tag(name));
			}
			numCalories = reader.ReadInt32();
		}

		public override string GetProgress(bool complete)
		{
			string text = "";
			for (int i = 0; i < foodProducers.Count; i++)
			{
				if (i != 0)
				{
					text += COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.PREPARED_SEPARATOR;
				}
				BuildingDef buildingDef = Assets.GetBuildingDef(foodProducers[i].Name);
				if (buildingDef != null)
				{
					text += buildingDef.Name;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CONSUME_ITEM, text);
		}
	}
}
