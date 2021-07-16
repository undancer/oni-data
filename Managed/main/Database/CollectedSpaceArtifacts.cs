using STRINGS;

namespace Database
{
	public class CollectedSpaceArtifacts : VictoryColonyAchievementRequirement
	{
		private const int REQUIRED_ARTIFACT_COUNT = 10;

		public override string GetProgress(bool complete)
		{
			return ((string)COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.COLLECT_SPACE_ARTIFACTS).Replace("{collectedCount}", GetStudiedSpaceArtifactCount().ToString()).Replace("{neededCount}", 10.ToString());
		}

		public override string Description()
		{
			return GetProgress(Success());
		}

		public override bool Success()
		{
			return ArtifactSelector.Instance.AnalyzedSpaceArtifactCount >= 10;
		}

		private int GetStudiedSpaceArtifactCount()
		{
			return ArtifactSelector.Instance.AnalyzedSpaceArtifactCount;
		}

		public override string Name()
		{
			return ((string)COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.REQUIREMENTS.STUDY_SPACE_ARTIFACTS).Replace("{artifactCount}", 10.ToString());
		}
	}
}
