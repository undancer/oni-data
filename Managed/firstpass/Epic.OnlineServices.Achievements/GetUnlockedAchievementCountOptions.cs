namespace Epic.OnlineServices.Achievements
{
	public class GetUnlockedAchievementCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId UserId { get; set; }
	}
}
