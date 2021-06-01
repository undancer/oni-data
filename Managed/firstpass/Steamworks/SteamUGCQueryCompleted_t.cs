using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(3401)]
	public struct SteamUGCQueryCompleted_t
	{
		public const int k_iCallback = 3401;

		public UGCQueryHandle_t m_handle;

		public EResult m_eResult;

		public uint m_unNumResultsReturned;

		public uint m_unTotalMatchingResults;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bCachedData;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_rgchNextCursor_;

		public string m_rgchNextCursor
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchNextCursor_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchNextCursor_, 256);
			}
		}
	}
}
