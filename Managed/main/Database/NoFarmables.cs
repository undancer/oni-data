using STRINGS;

namespace Database
{
	public class NoFarmables : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				foreach (PlantablePlot item in Components.PlantablePlots.GetItems(worldContainer.id))
				{
					if (!(item.Occupant != null))
					{
						continue;
					}
					Tag[] possibleDepositObjectTags = item.possibleDepositObjectTags;
					for (int i = 0; i < possibleDepositObjectTags.Length; i++)
					{
						if (possibleDepositObjectTags[i] != GameTags.DecorSeed)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public override bool Fail()
		{
			return !Success();
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_FARM_TILES;
		}
	}
}
