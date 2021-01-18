using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(331)]
	public struct GameOverlayActivated_t
	{
		public const int k_iCallback = 331;

		public byte m_bActive;
	}
}
