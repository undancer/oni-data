using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationSetPermissionLevelOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private OnlineSessionPermissionLevel m_PermissionLevel;

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

		public OnlineSessionPermissionLevel PermissionLevel
		{
			get
			{
				OnlineSessionPermissionLevel target = Helper.GetDefault<OnlineSessionPermissionLevel>();
				Helper.TryMarshalGet(m_PermissionLevel, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PermissionLevel, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
