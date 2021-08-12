using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetPortRangeOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private ushort m_Port;

		private ushort m_MaxAdditionalPortsToTry;

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

		public ushort Port
		{
			get
			{
				ushort target = Helper.GetDefault<ushort>();
				Helper.TryMarshalGet(m_Port, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Port, value);
			}
		}

		public ushort MaxAdditionalPortsToTry
		{
			get
			{
				ushort target = Helper.GetDefault<ushort>();
				Helper.TryMarshalGet(m_MaxAdditionalPortsToTry, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MaxAdditionalPortsToTry, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
