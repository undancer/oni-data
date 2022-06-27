using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Stats
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct StatInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Name;

		private long m_StartTime;

		private long m_EndTime;

		private int m_Value;

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

		public string Name
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Name, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Name, value);
			}
		}

		public DateTimeOffset? StartTime
		{
			get
			{
				DateTimeOffset? target = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(m_StartTime, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StartTime, value);
			}
		}

		public DateTimeOffset? EndTime
		{
			get
			{
				DateTimeOffset? target = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(m_EndTime, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_EndTime, value);
			}
		}

		public int Value
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_Value, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Value, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
