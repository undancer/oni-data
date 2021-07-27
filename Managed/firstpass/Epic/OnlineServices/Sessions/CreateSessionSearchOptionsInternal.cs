using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CreateSessionSearchOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_MaxSearchResults;

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

		public uint MaxSearchResults
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_MaxSearchResults, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MaxSearchResults, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
