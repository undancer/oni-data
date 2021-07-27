using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryLeaderboardUserScoresOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_UserIds;

		private uint m_UserIdsCount;

		private IntPtr m_StatInfo;

		private uint m_StatInfoCount;

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

		public ProductUserId[] UserIds
		{
			get
			{
				ProductUserId[] target = Helper.GetDefault<ProductUserId[]>();
				Helper.TryMarshalGet(m_UserIds, out target, m_UserIdsCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UserIds, value, out m_UserIdsCount);
			}
		}

		public UserScoresQueryStatInfoInternal[] StatInfo
		{
			get
			{
				UserScoresQueryStatInfoInternal[] target = Helper.GetDefault<UserScoresQueryStatInfoInternal[]>();
				Helper.TryMarshalGet(m_StatInfo, out target, m_StatInfoCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StatInfo, value, out m_StatInfoCount);
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
			Helper.TryMarshalDispose(ref m_UserIds);
			Helper.TryMarshalDispose(ref m_StatInfo);
		}
	}
}
