using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(1021)]
	public struct AppProofOfPurchaseKeyResponse_t
	{
		public const int k_iCallback = 1021;

		public EResult m_eResult;

		public uint m_nAppID;

		public uint m_cchKeyLength;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 240)]
		private byte[] m_rgchKey_;

		public string m_rgchKey
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchKey_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchKey_, 240);
			}
		}
	}
}
