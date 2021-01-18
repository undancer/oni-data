using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(341)]
	public struct DownloadClanActivityCountsResult_t
	{
		public const int k_iCallback = 341;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bSuccess;
	}
}
