using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Stats
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct IngestDataInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_StatName;

		private int m_IngestAmount;

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

		public string StatName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_StatName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StatName, value);
			}
		}

		public int IngestAmount
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_IngestAmount, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_IngestAmount, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
