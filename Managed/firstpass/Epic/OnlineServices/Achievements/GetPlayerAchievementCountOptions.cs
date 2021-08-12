namespace Epic.OnlineServices.Achievements
{
	public class GetPlayerAchievementCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId UserId { get; set; }
	}
}
