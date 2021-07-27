using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryOwnershipOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_CatalogItemIds;

		private uint m_CatalogItemIdCount;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CatalogNamespace;

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

		public string[] CatalogItemIds
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_CatalogItemIds, out target, m_CatalogItemIdCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CatalogItemIds, value, out m_CatalogItemIdCount);
			}
		}

		public string CatalogNamespace
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_CatalogNamespace, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CatalogNamespace, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_CatalogItemIds);
		}
	}
}
