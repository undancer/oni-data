using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PlayerAchievementInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AchievementId;

		private double m_Progress;

		private long m_UnlockTime;

		private int m_StatInfoCount;

		private IntPtr m_StatInfo;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Description;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_IconURL;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_FlavorText;

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

		public double Progress
		{
			get
			{
				double target = Helper.GetDefault<double>();
				Helper.TryMarshalGet(m_Progress, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Progress, value);
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
			set
			{
				Helper.TryMarshalSet(ref m_UnlockTime, value);
			}
		}

		public PlayerStatInfoInternal[] StatInfo
		{
			get
			{
				PlayerStatInfoInternal[] target = Helper.GetDefault<PlayerStatInfoInternal[]>();
				Helper.TryMarshalGet(m_StatInfo, out target, m_StatInfoCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StatInfo, value, out m_StatInfoCount);
			}
		}

		public string DisplayName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_DisplayName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DisplayName, value);
			}
		}

		public string Description
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Description, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Description, value);
			}
		}

		public string IconURL
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_IconURL, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_IconURL, value);
			}
		}

		public string FlavorText
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_FlavorText, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_FlavorText, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_StatInfo);
		}
	}
}
