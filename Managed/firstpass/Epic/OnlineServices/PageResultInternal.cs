using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PageResultInternal : IDisposable
	{
		private int m_StartIndex;

		private int m_Count;

		private int m_TotalCount;

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

		public int Count
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_Count, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Count, value);
			}
		}

		public int TotalCount
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_TotalCount, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_TotalCount, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
