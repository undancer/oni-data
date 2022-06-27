using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyLeaderboardRecordByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_LeaderboardRecordIndex;

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

		public uint LeaderboardRecordIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_LeaderboardRecordIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LeaderboardRecordIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
