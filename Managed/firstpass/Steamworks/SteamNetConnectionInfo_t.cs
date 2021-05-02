using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SteamNetConnectionInfo_t
	{
		public SteamNetworkingIdentity m_identityRemote;

		public long m_nUserData;

		public HSteamListenSocket m_hListenSocket;

		public SteamNetworkingIPAddr m_addrRemote;

		public ushort m__pad1;

		public SteamNetworkingPOPID m_idPOPRemote;

		public SteamNetworkingPOPID m_idPOPRelay;

		public ESteamNetworkingConnectionState m_eState;

		public int m_eEndReason;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private byte[] m_szEndDebug_;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private byte[] m_szConnectionDescription_;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public uint[] reserved;

		public string m_szEndDebug
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_szEndDebug_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_szEndDebug_, 128);
			}
		}

		public string m_szConnectionDescription
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_szConnectionDescription_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_szConnectionDescription_, 128);
			}
		}
	}
}
