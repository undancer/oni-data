using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnAchievementsUnlockedCallbackInfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		private IntPtr m_UserId;

		private uint m_AchievementsCount;

		private IntPtr m_AchievementIds;

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

		public string[] AchievementIds
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_AchievementIds, out target, m_AchievementsCount);
				return target;
			}
		}
	}
}
