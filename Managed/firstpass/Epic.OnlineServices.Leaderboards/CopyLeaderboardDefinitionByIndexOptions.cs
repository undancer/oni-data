namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardDefinitionByIndexOptions
	{
		public int ApiVersion => 1;

		public uint LeaderboardIndex
		{
			get;
			set;
		}
	}
}
