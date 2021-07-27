using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PresenceModificationSetStatusOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private Status m_Status;

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

		public Status Status
		{
			get
			{
				Status target = Helper.GetDefault<Status>();
				Helper.TryMarshalGet(m_Status, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Status, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
