using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(349)]
	public struct OverlayBrowserProtocolNavigation_t
	{
		public const int k_iCallback = 349;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		private byte[] rgchURI_;

		public string rgchURI
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(rgchURI_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, rgchURI_, 1024);
			}
		}
	}
}
