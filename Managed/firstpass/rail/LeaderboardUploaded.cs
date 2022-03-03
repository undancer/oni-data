namespace rail
{
	public class LeaderboardUploaded : EventBase
	{
		public int old_rank;

		public string leaderboard_name;

		public double score;

		public bool better_score;

		public int new_rank;
	}
}
