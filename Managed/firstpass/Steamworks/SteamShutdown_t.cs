using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 1)]
	[CallbackIdentity(704)]
	public struct SteamShutdown_t
	{
		public const int k_iCallback = 704;
	}
}
