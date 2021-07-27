using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AccountFeatureRestrictedInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_VerificationURI;

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

		public string VerificationURI
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_VerificationURI, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_VerificationURI, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
