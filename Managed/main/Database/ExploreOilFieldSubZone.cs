using STRINGS;

namespace Database
{
	public class ExploreOilFieldSubZone : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			return Game.Instance.savedInfo.discoveredOilField;
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ENTER_OIL_BIOME;
		}
	}
}
