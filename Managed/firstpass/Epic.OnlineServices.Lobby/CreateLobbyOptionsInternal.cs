using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CreateLobbyOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private uint m_MaxLobbyMembers;

		private LobbyPermissionLevel m_PermissionLevel;

		private int m_PresenceEnabled;

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

		public uint MaxLobbyMembers
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_MaxLobbyMembers, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MaxLobbyMembers, value);
			}
		}

		public LobbyPermissionLevel PermissionLevel
		{
			get
			{
				LobbyPermissionLevel target = Helper.GetDefault<LobbyPermissionLevel>();
				Helper.TryMarshalGet(m_PermissionLevel, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PermissionLevel, value);
			}
		}

		public bool PresenceEnabled
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_PresenceEnabled, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PresenceEnabled, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
