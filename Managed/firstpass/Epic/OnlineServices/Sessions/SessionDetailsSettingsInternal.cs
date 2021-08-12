using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionDetailsSettingsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_BucketId;

		private uint m_NumPublicConnections;

		private int m_AllowJoinInProgress;

		private OnlineSessionPermissionLevel m_PermissionLevel;

		private int m_InvitesAllowed;

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

		public string BucketId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_BucketId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_BucketId, value);
			}
		}

		public uint NumPublicConnections
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_NumPublicConnections, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_NumPublicConnections, value);
			}
		}

		public bool AllowJoinInProgress
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_AllowJoinInProgress, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AllowJoinInProgress, value);
			}
		}

		public OnlineSessionPermissionLevel PermissionLevel
		{
			get
			{
				OnlineSessionPermissionLevel target = Helper.GetDefault<OnlineSessionPermissionLevel>();
				Helper.TryMarshalGet(m_PermissionLevel, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PermissionLevel, value);
			}
		}

		public bool InvitesAllowed
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_InvitesAllowed, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_InvitesAllowed, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
