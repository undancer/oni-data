using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RedeemEntitlementsOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private uint m_EntitlementIdCount;

		private IntPtr m_EntitlementIds;

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

		public string[] EntitlementIds
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_EntitlementIds, out target, m_EntitlementIdCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EntitlementIds, value, out m_EntitlementIdCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_EntitlementIds);
		}
	}
}
