namespace Epic.OnlineServices.Leaderboards
{
	public class LeaderboardRecord
	{
		public int ApiVersion => 2;

		public ProductUserId UserId { get; set; }

		public uint Rank { get; set; }

		public int Score { get; set; }

		public string UserDisplayName { get; set; }
	}
}
