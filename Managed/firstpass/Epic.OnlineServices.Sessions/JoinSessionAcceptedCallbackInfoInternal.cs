using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct JoinSessionAcceptedCallbackInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private ulong m_UiEventId;

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

		public ulong UiEventId
		{
			get
			{
				ulong target = Helper.GetDefault<ulong>();
				Helper.TryMarshalGet(m_UiEventId, out target);
				return target;
			}
		}
	}
}
