using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationSetMaxPlayersOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_MaxPlayers;

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

		public uint MaxPlayers
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_MaxPlayers, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MaxPlayers, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
