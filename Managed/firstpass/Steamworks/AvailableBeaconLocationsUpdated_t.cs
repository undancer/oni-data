using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 1)]
	[CallbackIdentity(5305)]
	public struct AvailableBeaconLocationsUpdated_t
	{
		public const int k_iCallback = 5305;
	}
}
