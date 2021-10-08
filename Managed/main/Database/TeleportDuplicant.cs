using STRINGS;

namespace Database
{
	public class TeleportDuplicant : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TELEPORT_DUPLICANT;
		}

		public override bool Success()
		{
			foreach (WarpReceiver item in Components.WarpReceivers.Items)
			{
				if (item.Used)
				{
					return true;
				}
			}
			return false;
		}
	}
}
