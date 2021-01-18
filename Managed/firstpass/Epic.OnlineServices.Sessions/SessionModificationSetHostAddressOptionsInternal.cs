using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationSetHostAddressOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_HostAddress;

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

		public string HostAddress
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_HostAddress, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_HostAddress, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
