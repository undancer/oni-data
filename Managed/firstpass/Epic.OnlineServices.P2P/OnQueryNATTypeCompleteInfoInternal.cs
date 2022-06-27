using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnQueryNATTypeCompleteInfoInternal : ICallbackInfo
	{
		private Result m_ResultCode;

		private IntPtr m_ClientData;

		private NATType m_NATType;

		public Result ResultCode
		{
			get
			{
				Result target = Helper.GetDefault<Result>();
				Helper.TryMarshalGet(m_ResultCode, out target);
				return target;
			}
		}

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

		public NATType NATType
		{
			get
			{
				NATType target = Helper.GetDefault<NATType>();
				Helper.TryMarshalGet(m_NATType, out target);
				return target;
			}
		}
	}
}
