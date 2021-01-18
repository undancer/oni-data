using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Friends
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnFriendsUpdateInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		private IntPtr m_TargetUserId;

		private FriendsStatus m_PreviousStatus;

		private FriendsStatus m_CurrentStatus;

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

		public EpicAccountId TargetUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_TargetUserId, out target);
				return target;
			}
		}

		public FriendsStatus PreviousStatus
		{
			get
			{
				FriendsStatus target = Helper.GetDefault<FriendsStatus>();
				Helper.TryMarshalGet(m_PreviousStatus, out target);
				return target;
			}
		}

		public FriendsStatus CurrentStatus
		{
			get
			{
				FriendsStatus target = Helper.GetDefault<FriendsStatus>();
				Helper.TryMarshalGet(m_CurrentStatus, out target);
				return target;
			}
		}
	}
}
