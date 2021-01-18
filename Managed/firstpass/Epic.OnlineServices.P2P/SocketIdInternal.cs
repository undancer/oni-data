using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SocketIdInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
		private byte[] m_SocketName;

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

		public string SocketName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_SocketName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SocketName, value, 33);
			}
		}

		public void Dispose()
		{
		}
	}
}
