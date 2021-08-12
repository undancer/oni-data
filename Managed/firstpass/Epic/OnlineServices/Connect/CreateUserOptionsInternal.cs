using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CreateUserOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_ContinuanceToken;

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

		public ContinuanceToken ContinuanceToken
		{
			get
			{
				ContinuanceToken target = Helper.GetDefault<ContinuanceToken>();
				Helper.TryMarshalGet(m_ContinuanceToken, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ContinuanceToken, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
