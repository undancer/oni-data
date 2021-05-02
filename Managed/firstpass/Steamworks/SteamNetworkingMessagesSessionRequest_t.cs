using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(1251)]
	public struct SteamNetworkingMessagesSessionRequest_t
	{
		public const int k_iCallback = 1251;

		public SteamNetworkingIdentity m_identityRemote;
	}
}
