using System;

namespace Epic.OnlineServices.Leaderboards
{
	public class Definition
	{
		public int ApiVersion => 1;

		public string LeaderboardId
		{
			get;
			set;
		}

		public string StatName
		{
			get;
			set;
		}

		public LeaderboardAggregation Aggregation
		{
			get;
			set;
		}

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
