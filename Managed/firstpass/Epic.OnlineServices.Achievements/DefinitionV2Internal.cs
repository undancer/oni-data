using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DefinitionV2Internal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AchievementId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UnlockedDisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UnlockedDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedDisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_FlavorText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UnlockedIconURL;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedIconURL;

		private int m_IsHidden;

		private uint m_StatThresholdsCount;

		private IntPtr m_StatThresholds;

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

		public string UnlockedDisplayName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_UnlockedDisplayName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UnlockedDisplayName, value);
			}
		}

		public string UnlockedDescription
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_UnlockedDescription, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UnlockedDescription, value);
			}
		}

		public string LockedDisplayName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_LockedDisplayName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LockedDisplayName, value);
			}
		}

		public string LockedDescription
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_LockedDescription, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LockedDescription, value);
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

		public string UnlockedIconURL
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_UnlockedIconURL, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UnlockedIconURL, value);
			}
		}

		public string LockedIconURL
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_LockedIconURL, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LockedIconURL, value);
			}
		}

		public bool IsHidden
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_IsHidden, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_IsHidden, value);
			}
		}

		public StatThresholdsInternal[] StatThresholds
		{
			get
			{
				StatThresholdsInternal[] target = Helper.GetDefault<StatThresholdsInternal[]>();
				Helper.TryMarshalGet(m_StatThresholds, out target, m_StatThresholdsCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StatThresholds, value, out m_StatThresholdsCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_StatThresholds);
		}
	}
}
