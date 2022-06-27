using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct TransferDeviceIdAccountOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_PrimaryLocalUserId;

		private IntPtr m_LocalDeviceUserId;

		private IntPtr m_ProductUserIdToPreserve;

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

		public ProductUserId PrimaryLocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_PrimaryLocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PrimaryLocalUserId, value);
			}
		}

		public ProductUserId LocalDeviceUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalDeviceUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalDeviceUserId, value);
			}
		}

		public ProductUserId ProductUserIdToPreserve
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_ProductUserIdToPreserve, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductUserIdToPreserve, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
