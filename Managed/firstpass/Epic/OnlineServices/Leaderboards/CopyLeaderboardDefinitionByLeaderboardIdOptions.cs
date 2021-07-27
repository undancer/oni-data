namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardDefinitionByLeaderboardIdOptions
	{
		public int ApiVersion => 1;

		public string LeaderboardId { get; set; }
	}
}
