using STRINGS;

namespace Database
{
	public class RadBoltTravelDistance : ColonyAchievementRequirement
	{
		private int travelDistance;

		public RadBoltTravelDistance(int travelDistance)
		{
			this.travelDistance = travelDistance;
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.RADBOLT_TRAVEL, SaveGame.Instance.GetComponent<ColonyAchievementTracker>().radBoltTravelDistance, travelDistance);
		}

		public override bool Success()
		{
			return SaveGame.Instance.GetComponent<ColonyAchievementTracker>().radBoltTravelDistance > (float)travelDistance;
		}
	}
}
