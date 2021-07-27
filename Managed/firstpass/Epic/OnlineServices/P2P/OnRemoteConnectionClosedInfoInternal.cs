using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnRemoteConnectionClosedInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private IntPtr m_RemoteUserId;

		private IntPtr m_SocketId;

		private ConnectionClosedReason m_Reason;

		public object ClientData
		{
			get
			{
				object target = Helper.GetDefault<object>();
				Helper.TryMarshalGet(m_ClientData, out target);
				return target;
			}
		}

		public IntPtr ClientDataAddress => m_ClientData;

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
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
		}

		public SocketIdInternal? SocketId
		{
			get
			{
				SocketIdInternal? target = Helper.GetDefault<SocketIdInternal?>();
				Helper.TryMarshalGet(m_SocketId, out target);
				return target;
			}
		}

		public ConnectionClosedReason Reason
		{
			get
			{
				ConnectionClosedReason target = Helper.GetDefault<ConnectionClosedReason>();
				Helper.TryMarshalGet(m_Reason, out target);
				return target;
			}
		}
	}
}
