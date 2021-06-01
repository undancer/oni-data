using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(1222)]
	public struct SteamNetAuthenticationStatus_t
	{
		public const int k_iCallback = 1222;

		public ESteamNetworkingAvailability m_eAvail;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_debugMsg_;

		public string m_debugMsg
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_debugMsg_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_debugMsg_, 256);
			}
		}
	}
}
