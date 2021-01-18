using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyPlayerAchievementByAchievementIdOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_UserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AchievementId;

		public int ApiVersion
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ApiVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ApiVersion, value);
			}
		}

		public ProductUserId UserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_UserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UserId, value);
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
			set
			{
				Helper.TryMarshalSet(ref m_AchievementId, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
