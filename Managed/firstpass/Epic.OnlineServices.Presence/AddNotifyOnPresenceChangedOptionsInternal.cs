using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AddNotifyOnPresenceChangedOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

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

		public void Dispose()
		{
		}
	}
}
