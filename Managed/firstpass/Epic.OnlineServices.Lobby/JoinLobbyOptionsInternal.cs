using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct JoinLobbyOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LobbyDetailsHandle;

		private IntPtr m_LocalUserId;

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

		public LobbyDetails LobbyDetailsHandle
		{
			get
			{
				LobbyDetails target = Helper.GetDefault<LobbyDetails>();
				Helper.TryMarshalGet(m_LobbyDetailsHandle, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LobbyDetailsHandle, value);
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
