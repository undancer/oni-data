using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RegisterPlayersOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionName;

		private IntPtr m_PlayersToRegister;

		private uint m_PlayersToRegisterCount;

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

		public string SessionName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_SessionName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_SessionName, value);
			}
		}

		public ProductUserId[] PlayersToRegister
		{
			get
			{
				ProductUserId[] target = Helper.GetDefault<ProductUserId[]>();
				Helper.TryMarshalGet(m_PlayersToRegister, out target, m_PlayersToRegisterCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PlayersToRegister, value, out m_PlayersToRegisterCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_PlayersToRegister);
		}
	}
}
