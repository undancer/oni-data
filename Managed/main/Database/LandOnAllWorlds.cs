using STRINGS;

namespace Database
{
	public class LandOnAllWorlds : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			int num = 0;
			int num2 = 0;
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				if (!worldContainer.IsModuleInterior)
				{
					num++;
					if (worldContainer.IsDupeVisited || worldContainer.IsRoverVisted)
					{
						num2++;
					}
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAND_DUPES_ON_ALL_WORLDS, num2, num);
		}

		public override bool Success()
		{
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				if (!worldContainer.IsModuleInterior && !worldContainer.IsDupeVisited && !worldContainer.IsRoverVisted)
				{
					return false;
				}
			}
			return true;
		}
	}
}
