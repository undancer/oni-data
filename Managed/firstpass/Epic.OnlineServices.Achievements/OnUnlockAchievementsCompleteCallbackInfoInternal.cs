using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnUnlockAchievementsCompleteCallbackInfoInternal : ICallbackInfo
	{
		private Result m_ResultCode;

		private IntPtr m_ClientData;

		private IntPtr m_UserId;

		private uint m_AchievementsCount;

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

		public ProductUserId UserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_UserId, out target);
				return target;
			}
		}

		public uint AchievementsCount
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_AchievementsCount, out target);
				return target;
			}
		}
	}
}
