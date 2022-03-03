namespace rail
{
	public interface IRailLeaderboard : IRailComponent
	{
		string GetLeaderboardName();

		string GetLeaderboardDisplayName();

		int GetTotalEntriesCount();

		RailResult AsyncGetLeaderboard(string user_data);

		RailResult GetLeaderboardParameters(LeaderboardParameters param);

		IRailLeaderboardEntries CreateLeaderboardEntries();

		RailResult AsyncUploadLeaderboard(UploadLeaderboardParam update_param, string user_data);

		RailResult GetLeaderboardSortType(out int sort_type);

		RailResult GetLeaderboardDisplayType(out int display_type);

		RailResult AsyncAttachSpaceWork(SpaceWorkID spacework_id, string user_data);
	}
}
