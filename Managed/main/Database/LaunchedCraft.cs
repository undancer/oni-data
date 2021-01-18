using STRINGS;

namespace Database
{
	public class LaunchedCraft : ColonyAchievementRequirement
	{
		public override string GetProgress(bool completed)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAUNCHED_ROCKET;
		}

		public override bool Success()
		{
			foreach (Clustercraft clustercraft in Components.Clustercrafts)
			{
				if (clustercraft.Status == Clustercraft.CraftStatus.InFlight)
				{
					return true;
				}
			}
			return false;
		}
	}
}
