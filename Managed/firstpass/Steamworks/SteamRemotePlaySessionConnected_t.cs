using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(5701)]
	public struct SteamRemotePlaySessionConnected_t
	{
		public const int k_iCallback = 5701;

		public uint m_unSessionID;
	}
}
