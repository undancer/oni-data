using STRINGS;

namespace Database
{
	public class HarvestAHiveWithoutBeingStung : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.HARVEST_HIVE;
		}

		public override bool Success()
		{
			return SaveGame.Instance.GetComponent<ColonyAchievementTracker>().harvestAHiveWithoutGettingStung;
		}
	}
}
