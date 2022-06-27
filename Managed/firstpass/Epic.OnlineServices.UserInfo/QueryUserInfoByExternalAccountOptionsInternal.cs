using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UserInfo
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryUserInfoByExternalAccountOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ExternalAccountId;

		private ExternalAccountType m_AccountType;

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

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public string ExternalAccountId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ExternalAccountId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ExternalAccountId, value);
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

		public void Dispose()
		{
		}
	}
}
