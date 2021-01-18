namespace Epic.OnlineServices.Leaderboards
{
	public class CopyLeaderboardRecordByIndexOptions
	{
		public int ApiVersion => 2;

		public uint LeaderboardRecordIndex
		{
			get;
			set;
		}
	}
}
