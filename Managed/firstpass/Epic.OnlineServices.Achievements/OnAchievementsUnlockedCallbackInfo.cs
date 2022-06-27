namespace Epic.OnlineServices.Achievements
{
	public class OnAchievementsUnlockedCallbackInfo
	{
		public object ClientData { get; set; }

		public ProductUserId UserId { get; set; }

		public string[] AchievementIds { get; set; }
	}
}
