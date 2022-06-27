using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UI
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AddNotifyDisplaySettingsUpdatedOptionsInternal : IDisposable
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
