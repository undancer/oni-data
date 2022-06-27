using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LoginStatusChangedCallbackInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private LoginStatus m_PrevStatus;

		private LoginStatus m_CurrentStatus;

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

		public LoginStatus PrevStatus
		{
			get
			{
				LoginStatus target = Helper.GetDefault<LoginStatus>();
				Helper.TryMarshalGet(m_PrevStatus, out target);
				return target;
			}
		}

		public LoginStatus CurrentStatus
		{
			get
			{
				LoginStatus target = Helper.GetDefault<LoginStatus>();
				Helper.TryMarshalGet(m_CurrentStatus, out target);
				return target;
			}
		}
	}
}
