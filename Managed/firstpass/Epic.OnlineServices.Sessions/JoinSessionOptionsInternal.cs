using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct JoinSessionOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionName;

		private IntPtr m_SessionHandle;

		private IntPtr m_LocalUserId;

		private int m_PresenceEnabled;

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

		public string SessionName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_SessionName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SessionName, value);
			}
		}

		public SessionDetails SessionHandle
		{
			get
			{
				SessionDetails target = Helper.GetDefault<SessionDetails>();
				Helper.TryMarshalGet(m_SessionHandle, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SessionHandle, value);
			}
		}

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public bool PresenceEnabled
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_PresenceEnabled, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PresenceEnabled, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
