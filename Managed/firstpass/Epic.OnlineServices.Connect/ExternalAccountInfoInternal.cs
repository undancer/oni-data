using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ExternalAccountInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_ProductUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AccountId;

		private ExternalAccountType m_AccountIdType;

		private long m_LastLoginTime;

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

		public ProductUserId ProductUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_ProductUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductUserId, value);
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

		public ExternalAccountType AccountIdType
		{
			get
			{
				ExternalAccountType target = Helper.GetDefault<ExternalAccountType>();
				Helper.TryMarshalGet(m_AccountIdType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountIdType, value);
			}
		}

		public DateTimeOffset? LastLoginTime
		{
			get
			{
				DateTimeOffset? target = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(m_LastLoginTime, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LastLoginTime, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
