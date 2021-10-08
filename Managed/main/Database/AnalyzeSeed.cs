using STRINGS;

namespace Database
{
	public class AnalyzeSeed : ColonyAchievementRequirement
	{
		private string seedName;

		public AnalyzeSeed(string seedname)
		{
			seedName = seedname;
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ANALYZE_SEED, GameTagExtensions.ProperName(seedName));
		}

		public override bool Success()
		{
			return SaveGame.Instance.GetComponent<ColonyAchievementTracker>().analyzedSeeds.Contains(seedName);
		}
	}
}
