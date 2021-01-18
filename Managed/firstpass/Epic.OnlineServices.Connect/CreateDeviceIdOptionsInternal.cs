using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CreateDeviceIdOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DeviceModel;

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

		public string DeviceModel
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_DeviceModel, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DeviceModel, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
