using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PlayerStatInfoInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Name;

		private int m_CurrentValue;

		private int m_ThresholdValue;

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

		public int CurrentValue
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_CurrentValue, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_CurrentValue, value);
			}
		}

		public int ThresholdValue
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ThresholdValue, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ThresholdValue, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
