using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbySearchCopySearchResultByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_LobbyIndex;

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

		public uint LobbyIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_LobbyIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LobbyIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
