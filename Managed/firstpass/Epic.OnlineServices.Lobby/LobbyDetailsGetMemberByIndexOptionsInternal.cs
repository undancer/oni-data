using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyDetailsGetMemberByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_MemberIndex;

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

		public uint MemberIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_MemberIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MemberIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
