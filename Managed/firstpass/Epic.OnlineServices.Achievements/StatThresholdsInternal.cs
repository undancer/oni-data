using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct StatThresholdsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Name;

		private int m_Threshold;

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

		public int Threshold
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_Threshold, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Threshold, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
