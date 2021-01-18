using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LoginOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_Credentials;

		private IntPtr m_UserLoginInfo;

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

		public UserLoginInfoInternal? UserLoginInfo
		{
			get
			{
				UserLoginInfoInternal? target = Helper.GetDefault<UserLoginInfoInternal?>();
				Helper.TryMarshalGet(m_UserLoginInfo, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UserLoginInfo, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Credentials);
			Helper.TryMarshalDispose(ref m_UserLoginInfo);
		}
	}
}
