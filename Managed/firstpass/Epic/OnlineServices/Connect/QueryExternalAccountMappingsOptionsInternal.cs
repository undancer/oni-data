using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryExternalAccountMappingsOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private ExternalAccountType m_AccountIdType;

		private IntPtr m_ExternalAccountIds;

		private uint m_ExternalAccountIdCount;

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

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
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

		public string[] ExternalAccountIds
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_ExternalAccountIds, out target, m_ExternalAccountIdCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ExternalAccountIds, value, out m_ExternalAccountIdCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_ExternalAccountIds);
		}
	}
}
