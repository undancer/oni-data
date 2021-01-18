using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryLeaderboardDefinitionsOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private long m_StartTime;

		private long m_EndTime;

		public int ApiVersion
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ApiVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ApiVersion, value);
			}
		}

		public DateTimeOffset? StartTime
		{
			get
			{
				DateTimeOffset? target = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(m_StartTime, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StartTime, value);
			}
		}

		public DateTimeOffset? EndTime
		{
			get
			{
				DateTimeOffset? target = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(m_EndTime, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EndTime, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
