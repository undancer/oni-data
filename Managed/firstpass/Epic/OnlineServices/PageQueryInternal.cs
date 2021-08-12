using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PageQueryInternal : IDisposable
	{
		private int m_ApiVersion;

		private int m_StartIndex;

		private int m_MaxCount;

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

		public int StartIndex
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_StartIndex, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_StartIndex, value);
			}
		}

		public int MaxCount
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_MaxCount, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MaxCount, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
