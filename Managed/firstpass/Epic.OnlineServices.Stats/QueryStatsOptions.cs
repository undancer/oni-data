using System;

namespace Epic.OnlineServices.Stats
{
	public class QueryStatsOptions
	{
		public int ApiVersion => 2;

		public ProductUserId LocalUserId
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

		public string[] StatNames
		{
			get;
			set;
		}

		public ProductUserId TargetUserId
		{
			get;
			set;
		}
	}
}
