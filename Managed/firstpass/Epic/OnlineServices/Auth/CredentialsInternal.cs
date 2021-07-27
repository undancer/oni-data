using System;
using System.Runtime.InteropServices;
using Epic.OnlineServices.Connect;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CredentialsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Id;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Token;

		private LoginCredentialType m_Type;

		private IntPtr m_SystemAuthCredentialsOptions;

		private ExternalCredentialType m_ExternalType;

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

		public string Id
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Id, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Id, value);
			}
		}

		public string Token
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Token, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Token, value);
			}
		}

		public LoginCredentialType Type
		{
			get
			{
				LoginCredentialType target = Helper.GetDefault<LoginCredentialType>();
				Helper.TryMarshalGet(m_Type, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Type, value);
			}
		}

		public IntPtr SystemAuthCredentialsOptions
		{
			get
			{
				IntPtr target = Helper.GetDefault<IntPtr>();
				Helper.TryMarshalGet(m_SystemAuthCredentialsOptions, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SystemAuthCredentialsOptions, value);
			}
		}

		public ExternalCredentialType ExternalType
		{
			get
			{
				ExternalCredentialType target = Helper.GetDefault<ExternalCredentialType>();
				Helper.TryMarshalGet(m_ExternalType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ExternalType, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
