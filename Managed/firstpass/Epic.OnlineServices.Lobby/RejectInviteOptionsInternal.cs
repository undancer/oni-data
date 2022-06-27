using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RejectInviteOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_InviteId;

		private IntPtr m_LocalUserId;

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

		public string InviteId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_InviteId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_InviteId, value);
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

		public void Dispose()
		{
		}
	}
}
