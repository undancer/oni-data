using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct TransactionCopyEntitlementByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_EntitlementIndex;

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

		public uint EntitlementIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_EntitlementIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EntitlementIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
