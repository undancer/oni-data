using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(211)]
	public struct ComputeNewPlayerCompatibilityResult_t
	{
		public const int k_iCallback = 211;

		public EResult m_eResult;

		public int m_cPlayersThatDontLikeCandidate;

		public int m_cPlayersThatCandidateDoesntLike;

		public int m_cClanPlayersThatDontLikeCandidate;

		public CSteamID m_SteamIDCandidate;
	}
}
