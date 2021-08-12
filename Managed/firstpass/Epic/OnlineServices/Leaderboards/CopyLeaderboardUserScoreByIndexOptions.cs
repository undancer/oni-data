namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardUserScoreByIndexOptions
	{
		public int ApiVersion => 1;

		public uint LeaderboardUserScoreIndex { get; set; }

		public string StatName { get; set; }
	}
}
