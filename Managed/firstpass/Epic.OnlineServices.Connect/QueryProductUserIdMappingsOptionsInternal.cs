using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryProductUserIdMappingsOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private ExternalAccountType m_AccountIdType_DEPRECATED;

		private IntPtr m_ProductUserIds;

		private uint m_ProductUserIdCount;

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

		public ExternalAccountType AccountIdType_DEPRECATED
		{
			get
			{
				ExternalAccountType target = Helper.GetDefault<ExternalAccountType>();
				Helper.TryMarshalGet(m_AccountIdType_DEPRECATED, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountIdType_DEPRECATED, value);
			}
		}

		public ProductUserId[] ProductUserIds
		{
			get
			{
				ProductUserId[] target = Helper.GetDefault<ProductUserId[]>();
				Helper.TryMarshalGet(m_ProductUserIds, out target, m_ProductUserIdCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductUserIds, value, out m_ProductUserIdCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_ProductUserIds);
		}
	}
}
