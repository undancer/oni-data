using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CheckoutOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_OverrideCatalogNamespace;

		private uint m_EntryCount;

		private IntPtr m_Entries;

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

		public string OverrideCatalogNamespace
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_OverrideCatalogNamespace, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_OverrideCatalogNamespace, value);
			}
		}

		public CheckoutEntryInternal[] Entries
		{
			get
			{
				CheckoutEntryInternal[] target = Helper.GetDefault<CheckoutEntryInternal[]>();
				Helper.TryMarshalGet(m_Entries, out target, m_EntryCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Entries, value, out m_EntryCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Entries);
		}
	}
}
