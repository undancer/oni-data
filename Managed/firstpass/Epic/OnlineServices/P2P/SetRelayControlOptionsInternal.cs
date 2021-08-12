using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetRelayControlOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private RelayControl m_RelayControl;

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

		public RelayControl RelayControl
		{
			get
			{
				RelayControl target = Helper.GetDefault<RelayControl>();
				Helper.TryMarshalGet(m_RelayControl, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_RelayControl, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
