using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(1303)]
	public struct RemoteStorageAppSyncProgress_t
	{
		public const int k_iCallback = 1303;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
		private byte[] m_rgchCurrentFile_;

		public AppId_t m_nAppID;

		public uint m_uBytesTransferredThisChunk;

		public double m_dAppPercentComplete;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bUploading;

		public string m_rgchCurrentFile
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchCurrentFile_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchCurrentFile_, 260);
			}
		}
	}
}
