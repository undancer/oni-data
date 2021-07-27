using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct JoinGameAcceptedCallbackInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_JoinInfo;

		private IntPtr m_LocalUserId;

		private IntPtr m_TargetUserId;

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

		public string JoinInfo
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_JoinInfo, out target);
				return target;
			}
		}

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
		}

		public EpicAccountId TargetUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_TargetUserId, out target);
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
