using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LoginCallbackInfoInternal : ICallbackInfo
	{
		private Result m_ResultCode;

		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private IntPtr m_PinGrantInfo;

		private IntPtr m_ContinuanceToken;

		private IntPtr m_AccountFeatureRestrictedInfo;

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

		public PinGrantInfoInternal? PinGrantInfo
		{
			get
			{
				PinGrantInfoInternal? target = Helper.GetDefault<PinGrantInfoInternal?>();
				Helper.TryMarshalGet(m_PinGrantInfo, out target);
				return target;
			}
		}

		public ContinuanceToken ContinuanceToken
		{
			get
			{
				ContinuanceToken target = Helper.GetDefault<ContinuanceToken>();
				Helper.TryMarshalGet(m_ContinuanceToken, out target);
				return target;
			}
		}

		public AccountFeatureRestrictedInfoInternal? AccountFeatureRestrictedInfo
		{
			get
			{
				AccountFeatureRestrictedInfoInternal? target = Helper.GetDefault<AccountFeatureRestrictedInfoInternal?>();
				Helper.TryMarshalGet(m_AccountFeatureRestrictedInfo, out target);
				return target;
			}
		}
	}
}
