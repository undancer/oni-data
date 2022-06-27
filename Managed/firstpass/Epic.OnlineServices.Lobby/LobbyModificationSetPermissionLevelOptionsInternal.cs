using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyModificationSetPermissionLevelOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private LobbyPermissionLevel m_PermissionLevel;

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

		public void Dispose()
		{
		}
	}
}
