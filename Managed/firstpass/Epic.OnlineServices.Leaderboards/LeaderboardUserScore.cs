namespace Epic.OnlineServices.Leaderboards
{
	public class LeaderboardUserScore
	{
		public int ApiVersion => 1;

		public ProductUserId UserId { get; set; }

		public int Score { get; set; }
	}
}
