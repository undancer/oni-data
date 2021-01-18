using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationSetInvitesAllowedOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private int m_InvitesAllowed;

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

		public bool InvitesAllowed
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_InvitesAllowed, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_InvitesAllowed, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
