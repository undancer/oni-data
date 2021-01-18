using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryEntitlementsOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_EntitlementNames;

		private uint m_EntitlementNameCount;

		private int m_IncludeRedeemed;

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

		public string[] EntitlementNames
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_EntitlementNames, out target, m_EntitlementNameCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EntitlementNames, value, out m_EntitlementNameCount);
			}
		}

		public bool IncludeRedeemed
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_IncludeRedeemed, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_IncludeRedeemed, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_EntitlementNames);
		}
	}
}
