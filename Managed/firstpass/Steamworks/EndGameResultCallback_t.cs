using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(5215)]
	public struct EndGameResultCallback_t
	{
		public const int k_iCallback = 5215;

		public EResult m_eResult;

		public ulong ullUniqueGameID;
	}
}
