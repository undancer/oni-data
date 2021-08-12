using STRINGS;

namespace Database
{
	public class OpenTemporalTear : VictoryColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.OPEN_TEMPORAL_TEAR;
		}

		public override string Description()
		{
			return GetProgress(Success());
		}

		public override bool Success()
		{
			return ClusterManager.Instance.GetComponent<ClusterPOIManager>().IsTemporalTearOpen();
		}

		public override string Name()
		{
			return COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.OPEN_TEMPORAL_TEAR;
		}
	}
}
