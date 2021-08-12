using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyDetailsInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LobbyId;

		private IntPtr m_LobbyOwnerUserId;

		private LobbyPermissionLevel m_PermissionLevel;

		private uint m_AvailableSlots;

		private uint m_MaxMembers;

		private int m_AllowInvites;

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

		public string LobbyId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_LobbyId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LobbyId, value);
			}
		}

		public ProductUserId LobbyOwnerUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LobbyOwnerUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LobbyOwnerUserId, value);
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

		public uint AvailableSlots
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_AvailableSlots, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AvailableSlots, value);
			}
		}

		public uint MaxMembers
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_MaxMembers, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MaxMembers, value);
			}
		}

		public bool AllowInvites
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_AllowInvites, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AllowInvites, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
