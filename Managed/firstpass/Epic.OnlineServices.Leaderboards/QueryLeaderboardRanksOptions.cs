namespace Epic.OnlineServices.Leaderboards
{
	public class QueryLeaderboardRanksOptions
	{
		public int ApiVersion => 1;

		public string LeaderboardId
		{
			get;
			set;
		}
	}
}
