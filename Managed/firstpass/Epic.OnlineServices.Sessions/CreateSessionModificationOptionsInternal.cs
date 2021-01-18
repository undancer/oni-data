using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CreateSessionModificationOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_BucketId;

		private uint m_MaxPlayers;

		private IntPtr m_LocalUserId;

		private int m_PresenceEnabled;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionId;

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

		public string BucketId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_BucketId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_BucketId, value);
			}
		}

		public uint MaxPlayers
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_MaxPlayers, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MaxPlayers, value);
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

		public void Dispose()
		{
		}
	}
}
