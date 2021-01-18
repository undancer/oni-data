using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Metrics
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct EndPlayerSessionOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private EndPlayerSessionOptionsAccountIdInternal m_AccountId;

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

		public EndPlayerSessionOptionsAccountIdInternal AccountId
		{
			get
			{
				EndPlayerSessionOptionsAccountIdInternal target = Helper.GetDefault<EndPlayerSessionOptionsAccountIdInternal>();
				Helper.TryMarshalGet(m_AccountId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountId, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_AccountId);
		}
	}
}
