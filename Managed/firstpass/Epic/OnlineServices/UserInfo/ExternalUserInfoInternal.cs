using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UserInfo
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ExternalUserInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		private ExternalAccountType m_AccountType;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AccountId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DisplayName;

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

		public ExternalAccountType AccountType
		{
			get
			{
				ExternalAccountType target = Helper.GetDefault<ExternalAccountType>();
				Helper.TryMarshalGet(m_AccountType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountType, value);
			}
		}

		public string AccountId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_AccountId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountId, value);
			}
		}

		public string DisplayName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_DisplayName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DisplayName, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
