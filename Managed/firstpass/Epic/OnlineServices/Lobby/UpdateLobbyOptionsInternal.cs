using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UpdateLobbyOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LobbyModificationHandle;

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

		public LobbyModification LobbyModificationHandle
		{
			get
			{
				LobbyModification target = Helper.GetDefault<LobbyModification>();
				Helper.TryMarshalGet(m_LobbyModificationHandle, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LobbyModificationHandle, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
