using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionDetailsInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_HostAddress;

		private uint m_NumOpenPublicConnections;

		private IntPtr m_Settings;

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

		public string SessionId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_SessionId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SessionId, value);
			}
		}

		public string HostAddress
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_HostAddress, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_HostAddress, value);
			}
		}

		public uint NumOpenPublicConnections
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_NumOpenPublicConnections, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_NumOpenPublicConnections, value);
			}
		}

		public SessionDetailsSettingsInternal? Settings
		{
			get
			{
				SessionDetailsSettingsInternal? target = Helper.GetDefault<SessionDetailsSettingsInternal?>();
				Helper.TryMarshalGet(m_Settings, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Settings, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Settings);
		}
	}
}
