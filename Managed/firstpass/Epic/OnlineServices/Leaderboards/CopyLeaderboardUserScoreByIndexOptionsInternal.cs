using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyLeaderboardUserScoreByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_LeaderboardUserScoreIndex;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_StatName;

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

		public uint LeaderboardUserScoreIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_LeaderboardUserScoreIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LeaderboardUserScoreIndex, value);
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

		public void Dispose()
		{
		}
	}
}
