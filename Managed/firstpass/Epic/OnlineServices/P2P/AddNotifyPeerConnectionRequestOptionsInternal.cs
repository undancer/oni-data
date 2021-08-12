using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AddNotifyPeerConnectionRequestOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_SocketId;

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

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_SocketId);
		}
	}
}
