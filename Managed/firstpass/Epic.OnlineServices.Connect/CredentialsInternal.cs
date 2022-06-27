using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CredentialsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Token;

		private ExternalCredentialType m_Type;

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

		public ExternalCredentialType Type
		{
			get
			{
				ExternalCredentialType target = Helper.GetDefault<ExternalCredentialType>();
				Helper.TryMarshalGet(m_Type, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Type, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
