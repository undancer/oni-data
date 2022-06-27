using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DefinitionInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AchievementId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Description;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedDisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_HiddenDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CompletionDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UnlockedIconId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedIconId;

		private int m_IsHidden;

		private int m_StatThresholdsCount;

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

		public string HiddenDescription
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_HiddenDescription, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_HiddenDescription, value);
			}
		}

		public string CompletionDescription
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_CompletionDescription, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CompletionDescription, value);
			}
		}

		public string UnlockedIconId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_UnlockedIconId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UnlockedIconId, value);
			}
		}

		public string LockedIconId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_LockedIconId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LockedIconId, value);
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
