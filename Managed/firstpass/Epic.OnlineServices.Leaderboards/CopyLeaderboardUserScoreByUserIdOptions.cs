namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardUserScoreByUserIdOptions
	{
		public int ApiVersion => 1;

		public ProductUserId UserId
		{
			get;
			set;
		}

		public string StatName
		{
			get;
			set;
		}
	}
}
