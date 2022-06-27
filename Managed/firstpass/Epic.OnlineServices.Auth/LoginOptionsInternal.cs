using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LoginOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_Credentials;

		private AuthScopeFlags m_ScopeFlags;

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

		public CredentialsInternal? Credentials
		{
			get
			{
				CredentialsInternal? target = Helper.GetDefault<CredentialsInternal?>();
				Helper.TryMarshalGet(m_Credentials, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Credentials, value);
			}
		}

		public AuthScopeFlags ScopeFlags
		{
			get
			{
				AuthScopeFlags target = Helper.GetDefault<AuthScopeFlags>();
				Helper.TryMarshalGet(m_ScopeFlags, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ScopeFlags, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Credentials);
		}
	}
}
