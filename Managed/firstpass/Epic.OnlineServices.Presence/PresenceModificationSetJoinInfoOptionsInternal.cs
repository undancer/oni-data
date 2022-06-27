using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PresenceModificationSetJoinInfoOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_JoinInfo;

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

		public string JoinInfo
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_JoinInfo, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_JoinInfo, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
