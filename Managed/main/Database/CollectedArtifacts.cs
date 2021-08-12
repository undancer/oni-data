using STRINGS;

namespace Database
{
	public class CollectedArtifacts : VictoryColonyAchievementRequirement
	{
		private const int REQUIRED_ARTIFACT_COUNT = 10;

		public override string GetProgress(bool complete)
		{
			return ((string)COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.COLLECT_ARTIFACTS).Replace("{collectedCount}", GetStudiedArtifactCount().ToString()).Replace("{neededCount}", 10.ToString());
		}

		public override string Description()
		{
			return GetProgress(Success());
		}

		public override bool Success()
		{
			return ArtifactSelector.Instance.AnalyzedArtifactCount >= 10;
		}

		private int GetStudiedArtifactCount()
		{
			return ArtifactSelector.Instance.AnalyzedArtifactCount;
		}

		public override string Name()
		{
			return ((string)COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.REQUIREMENTS.STUDY_ARTIFACTS).Replace("{artifactCount}", 10.ToString());
		}
	}
}
