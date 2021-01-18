using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionModificationSetJoinInProgressAllowedOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private int m_AllowJoinInProgress;

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

		public bool AllowJoinInProgress
		{
			get
			{
				bool target = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(m_AllowJoinInProgress, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AllowJoinInProgress, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
