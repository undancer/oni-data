using STRINGS;

namespace Database
{
	public class CureDisease : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			return Game.Instance.savedInfo.curedDisease;
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CURED_DISEASE;
		}
	}
}
