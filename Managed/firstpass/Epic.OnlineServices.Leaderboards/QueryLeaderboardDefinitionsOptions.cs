using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class QueryLeaderboardDefinitionsOptions
	{
		public int ApiVersion => 1;

		public DateTimeOffset? StartTime
		{
			get;
			set;
		}

		public DateTimeOffset? EndTime
		{
			get;
			set;
		}
	}
}
