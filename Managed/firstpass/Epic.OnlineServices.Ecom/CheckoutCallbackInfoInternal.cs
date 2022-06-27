using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CheckoutCallbackInfoInternal : ICallbackInfo
	{
		private Result m_ResultCode;

		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_TransactionId;

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

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
		}

		public string TransactionId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_TransactionId, out target);
				return target;
			}
		}
	}
}
