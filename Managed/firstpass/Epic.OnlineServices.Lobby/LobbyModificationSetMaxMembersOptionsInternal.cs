using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyModificationSetMaxMembersOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_MaxMembers;

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

		public void Dispose()
		{
		}
	}
}
