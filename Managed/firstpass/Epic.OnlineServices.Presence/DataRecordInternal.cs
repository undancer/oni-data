using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DataRecordInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Key;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Value;

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

		public string Key
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Key, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Key, value);
			}
		}

		public string Value
		{
			get
			{
				string target = Helper.GetDefault<string>();
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
