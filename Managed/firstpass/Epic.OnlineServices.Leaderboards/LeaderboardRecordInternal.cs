using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LeaderboardRecordInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_UserId;

		private uint m_Rank;

		private int m_Score;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UserDisplayName;

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

		public ProductUserId UserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_UserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UserId, value);
			}
		}

		public uint Rank
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_Rank, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Rank, value);
			}
		}

		public int Score
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_Score, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Score, value);
			}
		}

		public string UserDisplayName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_UserDisplayName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UserDisplayName, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
