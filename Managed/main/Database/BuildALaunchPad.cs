using STRINGS;

namespace Database
{
	public class BuildALaunchPad : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILD_A_LAUNCHPAD;
		}

		public override bool Success()
		{
			foreach (LaunchPad item in Components.LaunchPads.Items)
			{
				WorldContainer myWorld = item.GetMyWorld();
				if (!myWorld.IsStartWorld && Components.WarpReceivers.GetWorldItems(myWorld.id).Count == 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
