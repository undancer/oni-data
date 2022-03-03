namespace rail
{
	public class PlayerAchievementStored : EventBase
	{
		public bool group_achievement;

		public string achievement_name;

		public uint current_progress;

		public uint max_progress;
	}
}
