namespace Epic.OnlineServices.Leaderboards
{
	public class GetLeaderboardUserScoreCountOptions
	{
		public int ApiVersion => 1;

		public string StatName { get; set; }
	}
}
