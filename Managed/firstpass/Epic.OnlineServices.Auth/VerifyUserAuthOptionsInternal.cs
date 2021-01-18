using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct VerifyUserAuthOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_AuthToken;

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

		public TokenInternal? AuthToken
		{
			get
			{
				TokenInternal? target = Helper.GetDefault<TokenInternal?>();
				Helper.TryMarshalGet(m_AuthToken, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AuthToken, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_AuthToken);
		}
	}
}
