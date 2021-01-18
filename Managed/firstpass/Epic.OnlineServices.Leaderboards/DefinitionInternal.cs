using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DefinitionInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LeaderboardId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_StatName;

		private LeaderboardAggregation m_Aggregation;

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

		public string LeaderboardId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_LeaderboardId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LeaderboardId, value);
			}
		}

		public string StatName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_StatName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StatName, value);
			}
		}

		public LeaderboardAggregation Aggregation
		{
			get
			{
				LeaderboardAggregation target = Helper.GetDefault<LeaderboardAggregation>();
				Helper.TryMarshalGet(m_Aggregation, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Aggregation, value);
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
