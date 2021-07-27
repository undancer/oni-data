using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SendPacketOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_RemoteUserId;

		private IntPtr m_SocketId;

		private byte m_Channel;

		private uint m_DataLengthBytes;

		private IntPtr m_Data;

		private int m_AllowDelayedDelivery;

		private PacketReliability m_Reliability;

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

		public ProductUserId RemoteUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_RemoteUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_RemoteUserId, value);
			}
		}

		public SocketIdInternal? SocketId
		{
			get
			{
				SocketIdInternal? target = Helper.GetDefault<SocketIdInternal?>();
				Helper.TryMarshalGet(m_SocketId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SocketId, value);
			}
		}

		public byte Channel
		{
			get
			{
				byte target = Helper.GetDefault<byte>();
				Helper.TryMarshalGet(m_Channel, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Channel, value);
			}
		}

		public byte[] Data
		{
			get
			{
				byte[] target = Helper.GetDefault<byte[]>();
				Helper.TryMarshalGet(m_Data, out target, m_DataLengthBytes);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Data, value, out m_DataLengthBytes);
			}
		}

		public bool AllowDelayedDelivery
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_AllowDelayedDelivery, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AllowDelayedDelivery, value);
			}
		}

		public PacketReliability Reliability
		{
			get
			{
				PacketReliability target = Helper.GetDefault<PacketReliability>();
				Helper.TryMarshalGet(m_Reliability, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Reliability, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_SocketId);
			Helper.TryMarshalDispose(ref m_Data);
		}
	}
}
