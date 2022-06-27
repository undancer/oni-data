using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ClientCredentialsInternal : IDisposable
	{
		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ClientId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ClientSecret;

		public string ClientId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ClientId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ClientId, value);
			}
		}

		public string ClientSecret
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ClientSecret, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ClientSecret, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
