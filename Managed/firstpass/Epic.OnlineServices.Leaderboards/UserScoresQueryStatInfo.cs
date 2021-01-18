namespace Epic.OnlineServices.Leaderboards
{
	public class UserScoresQueryStatInfo
	{
		public int ApiVersion => 1;

		public string StatName
		{
			get;
			set;
		}

		public LeaderboardAggregation Aggregation
		{
			get;
			set;
		}
	}
}
