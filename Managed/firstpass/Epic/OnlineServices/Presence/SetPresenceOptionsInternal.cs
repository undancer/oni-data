using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetPresenceOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_PresenceModificationHandle;

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

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public PresenceModification PresenceModificationHandle
		{
			get
			{
				PresenceModification target = Helper.GetDefault<PresenceModification>();
				Helper.TryMarshalGet(m_PresenceModificationHandle, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PresenceModificationHandle, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
