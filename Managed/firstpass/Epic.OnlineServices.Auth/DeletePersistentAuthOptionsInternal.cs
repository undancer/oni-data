using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DeletePersistentAuthOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_RefreshToken;

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

		public string RefreshToken
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_RefreshToken, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_RefreshToken, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
