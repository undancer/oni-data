using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class QueryLeaderboardUserScoresOptions
	{
		public int ApiVersion => 1;

		public ProductUserId[] UserIds { get; set; }

		public UserScoresQueryStatInfo[] StatInfo { get; set; }

		public DateTimeOffset? StartTime { get; set; }

		public DateTimeOffset? EndTime { get; set; }
	}
}
