using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PresenceModificationSetDataOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private int m_RecordsCount;

		private IntPtr m_Records;

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

		public DataRecordInternal[] Records
		{
			get
			{
				DataRecordInternal[] target = Helper.GetDefault<DataRecordInternal[]>();
				Helper.TryMarshalGet(m_Records, out target, m_RecordsCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Records, value, out m_RecordsCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Records);
		}
	}
}
