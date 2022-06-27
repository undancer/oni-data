using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyLeaderboardDefinitionByLeaderboardIdOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LeaderboardId;

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

		public void Dispose()
		{
		}
	}
}
