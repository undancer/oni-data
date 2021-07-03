using STRINGS;

namespace Database
{
	public class CollectedArtifacts : VictoryColonyAchievementRequirement
	{
		private const int REQUIRED_ARTIFACT_COUNT = 20;

		public override string GetProgress(bool complete)
		{
			string text = COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.COLLECT_ARTIFACTS;
			text = text.Replace("{collectedCount}", GetStudiedArtifactCount().ToString());
			return text.Replace("{neededCount}", 20.ToString());
		}

		public override string Description()
		{
			return GetProgress(Success());
		}

		public override bool Success()
		{
			return ArtifactSelector.Instance.AnalyzedArtifactCount >= 20;
		}

		private int GetStudiedArtifactCount()
		{
			return ArtifactSelector.Instance.AnalyzedArtifactCount;
		}

		public override string Name()
		{
			string text = COLONY_ACHIEVEMENTS.CONQUER_CLUSTER.REQUIREMENTS.STUDY_ARTIFACTS;
			return text.Replace("{artifactCount}", 20.ToString());
		}
	}
}
