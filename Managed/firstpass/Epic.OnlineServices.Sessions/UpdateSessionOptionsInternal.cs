using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UpdateSessionOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_SessionModificationHandle;

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

		public SessionModification SessionModificationHandle
		{
			get
			{
				SessionModification target = Helper.GetDefault<SessionModification>();
				Helper.TryMarshalGet(m_SessionModificationHandle, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SessionModificationHandle, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
