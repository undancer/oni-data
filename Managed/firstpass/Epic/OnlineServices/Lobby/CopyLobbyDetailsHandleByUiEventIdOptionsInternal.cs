using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CopyLobbyDetailsHandleByUiEventIdOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private ulong m_UiEventId;

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

		public ulong UiEventId
		{
			get
			{
				ulong target = Helper.GetDefault<ulong>();
				Helper.TryMarshalGet(m_UiEventId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UiEventId, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
