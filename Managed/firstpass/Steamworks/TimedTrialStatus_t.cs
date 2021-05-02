using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(1030)]
	public struct TimedTrialStatus_t
	{
		public const int k_iCallback = 1030;

		public AppId_t m_unAppID;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bIsOffline;

		public uint m_unSecondsAllowed;

		public uint m_unSecondsPlayed;
	}
}
