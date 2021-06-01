using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(1252)]
	public struct SteamNetworkingMessagesSessionFailed_t
	{
		public const int k_iCallback = 1252;

		public SteamNetConnectionInfo_t m_info;
	}
}
