using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct TokenInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_App;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ClientId;

		private IntPtr m_AccountId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AccessToken;

		private double m_ExpiresIn;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ExpiresAt;

		private AuthTokenType m_AuthType;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_RefreshToken;

		private double m_RefreshExpiresIn;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_RefreshExpiresAt;

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

		public string App
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_App, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_App, value);
			}
		}

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

		public EpicAccountId AccountId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_AccountId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountId, value);
			}
		}

		public string AccessToken
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_AccessToken, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccessToken, value);
			}
		}

		public double ExpiresIn
		{
			get
			{
				double target = Helper.GetDefault<double>();
				Helper.TryMarshalGet(m_ExpiresIn, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ExpiresIn, value);
			}
		}

		public string ExpiresAt
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ExpiresAt, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ExpiresAt, value);
			}
		}

		public AuthTokenType AuthType
		{
			get
			{
				AuthTokenType target = Helper.GetDefault<AuthTokenType>();
				Helper.TryMarshalGet(m_AuthType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AuthType, value);
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

		public double RefreshExpiresIn
		{
			get
			{
				double target = Helper.GetDefault<double>();
				Helper.TryMarshalGet(m_RefreshExpiresIn, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_RefreshExpiresIn, value);
			}
		}

		public string RefreshExpiresAt
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_RefreshExpiresAt, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_RefreshExpiresAt, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
