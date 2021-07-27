using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.UI
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AcknowledgeEventIdOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private ulong m_UiEventId;

		private Result m_Result;

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

		public ulong UiEventId
		{
			get
			{
				ulong target = Helper.GetDefault<ulong>();
				Helper.TryMarshalGet(m_UiEventId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UiEventId, value);
			}
		}

		public Result Result
		{
			get
			{
				Result target = Helper.GetDefault<Result>();
				Helper.TryMarshalGet(m_Result, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Result, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
