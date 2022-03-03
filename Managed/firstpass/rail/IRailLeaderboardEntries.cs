namespace rail
{
	public interface IRailLeaderboardEntries : IRailComponent
	{
		RailID GetRailID();

		string GetLeaderboardName();

		RailResult AsyncRequestLeaderboardEntries(RailID player, RequestLeaderboardEntryParam param, string user_data);

		RequestLeaderboardEntryParam GetEntriesParam();

		int GetEntriesCount();

		RailResult GetLeaderboardEntry(int index, LeaderboardEntry leaderboard_entry);
	}
}
