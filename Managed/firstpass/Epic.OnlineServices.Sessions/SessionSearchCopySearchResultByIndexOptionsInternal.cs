using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionSearchCopySearchResultByIndexOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_SessionIndex;

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

		public uint SessionIndex
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_SessionIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SessionIndex, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
