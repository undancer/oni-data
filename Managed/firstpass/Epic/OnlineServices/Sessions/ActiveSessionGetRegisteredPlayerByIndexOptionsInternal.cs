using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ActiveSessionGetRegisteredPlayerByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_PlayerIndex;

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

		public uint PlayerIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_PlayerIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PlayerIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
