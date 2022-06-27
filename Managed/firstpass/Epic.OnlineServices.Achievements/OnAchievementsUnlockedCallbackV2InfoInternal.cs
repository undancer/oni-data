using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnAchievementsUnlockedCallbackV2InfoInternal : ICallbackInfo
	{
		private IntPtr m_ClientData;

		private IntPtr m_UserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AchievementId;

		private long m_UnlockTime;

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

		public string AchievementId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_AchievementId, out target);
				return target;
			}
		}

		public DateTimeOffset? UnlockTime
		{
			get
			{
				DateTimeOffset? target = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(m_UnlockTime, out target);
				return target;
			}
		}
	}
}
