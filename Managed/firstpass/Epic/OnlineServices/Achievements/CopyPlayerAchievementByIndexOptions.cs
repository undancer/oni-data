namespace Epic.OnlineServices.Achievements
{
	public class CopyPlayerAchievementByIndexOptions
	{
		public int ApiVersion => 1;

		public ProductUserId UserId { get; set; }

		public uint AchievementIndex { get; set; }
	}
}
