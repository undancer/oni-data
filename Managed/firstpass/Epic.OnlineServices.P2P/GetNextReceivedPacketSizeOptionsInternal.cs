using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GetNextReceivedPacketSizeOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_RequestedChannel;

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

		public byte? RequestedChannel
		{
			get
			{
				byte? target = Helper.GetDefault<byte?>();
				Helper.TryMarshalGet(m_RequestedChannel, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_RequestedChannel, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_RequestedChannel);
		}
	}
}
