using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Stats
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyStatByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_TargetUserId;

		private uint m_StatIndex;

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

		public ProductUserId TargetUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_TargetUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_TargetUserId, value);
			}
		}

		public uint StatIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_StatIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StatIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
