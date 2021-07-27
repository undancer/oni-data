using System;

namespace Epic.OnlineServices.Stats
{
	public class Stat
	{
		public int ApiVersion => 1;

		public string Name { get; set; }

		public DateTimeOffset? StartTime { get; set; }

		public DateTimeOffset? EndTime { get; set; }

		public int Value { get; set; }
	}
}
