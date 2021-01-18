using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyAchievementDefinitionV2ByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_AchievementIndex;

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

		public uint AchievementIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_AchievementIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AchievementIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
