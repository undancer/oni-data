using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SteamPartyBeaconLocation_t
	{
		public ESteamPartyBeaconLocationType m_eType;

		public ulong m_ulLocationID;
	}
}
