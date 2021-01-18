namespace Epic.OnlineServices.Achievements
{
	public class UnlockAchievementsOptions
	{
		public int ApiVersion => 1;

		public ProductUserId UserId
		{
			get;
			set;
		}

		public string[] AchievementIds
		{
			get;
			set;
		}
	}
}
