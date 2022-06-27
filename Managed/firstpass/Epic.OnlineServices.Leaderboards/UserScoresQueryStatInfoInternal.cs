using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UserScoresQueryStatInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_StatName;

		private LeaderboardAggregation m_Aggregation;

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

		public void Dispose()
		{
		}
	}
}
