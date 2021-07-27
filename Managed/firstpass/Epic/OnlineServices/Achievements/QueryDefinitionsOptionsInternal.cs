using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryDefinitionsOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_EpicUserId_DEPRECATED;

		private IntPtr m_HiddenAchievementIds_DEPRECATED;

		private uint m_HiddenAchievementsCount_DEPRECATED;

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

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public EpicAccountId EpicUserId_DEPRECATED
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_EpicUserId_DEPRECATED, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EpicUserId_DEPRECATED, value);
			}
		}

		public string[] HiddenAchievementIds_DEPRECATED
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_HiddenAchievementIds_DEPRECATED, out target, m_HiddenAchievementsCount_DEPRECATED);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_HiddenAchievementIds_DEPRECATED, value, out m_HiddenAchievementsCount_DEPRECATED);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_HiddenAchievementIds_DEPRECATED);
		}
	}
}
