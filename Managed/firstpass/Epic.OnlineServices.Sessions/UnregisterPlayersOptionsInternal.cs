using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UnregisterPlayersOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SessionName;

		private IntPtr m_PlayersToUnregister;

		private uint m_PlayersToUnregisterCount;

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

		public ProductUserId[] PlayersToUnregister
		{
			get
			{
				ProductUserId[] target = Helper.GetDefault<ProductUserId[]>();
				Helper.TryMarshalGet(m_PlayersToUnregister, out target, m_PlayersToUnregisterCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_PlayersToUnregister, value, out m_PlayersToUnregisterCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_PlayersToUnregister);
		}
	}
}
