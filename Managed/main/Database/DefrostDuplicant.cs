using STRINGS;

namespace Database
{
	public class DefrostDuplicant : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.DEFROST_DUPLICANT;
		}

		public override bool Success()
		{
			return SaveGame.Instance.GetComponent<ColonyAchievementTracker>().defrostedDuplicant;
		}
	}
}
