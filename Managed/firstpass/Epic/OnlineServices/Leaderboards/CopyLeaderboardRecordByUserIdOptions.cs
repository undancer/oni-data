namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardRecordByUserIdOptions
	{
		public int ApiVersion => 2;

		public ProductUserId UserId { get; set; }
	}
}
