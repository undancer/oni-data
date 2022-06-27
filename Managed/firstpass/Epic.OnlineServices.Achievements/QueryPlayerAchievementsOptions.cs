namespace Epic.OnlineServices.Achievements
{
	public class QueryPlayerAchievementsOptions
	{
		public int ApiVersion => 1;

		public ProductUserId UserId { get; set; }
	}
}
