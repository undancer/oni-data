using STRINGS;

namespace Database
{
	public class BlockedCometWithBunkerDoor : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			return Game.Instance.savedInfo.blockedCometWithBunkerDoor;
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BLOCKED_A_COMET;
		}
	}
}
