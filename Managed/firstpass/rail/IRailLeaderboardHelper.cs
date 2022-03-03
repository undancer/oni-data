namespace rail
{
	public interface IRailLeaderboardHelper
	{
		IRailLeaderboard OpenLeaderboard(string leaderboard_name);

		IRailLeaderboard AsyncCreateLeaderboard(string leaderboard_name, LeaderboardSortType sort_type, LeaderboardDisplayType display_type, string user_data, out RailResult result);
	}
}
