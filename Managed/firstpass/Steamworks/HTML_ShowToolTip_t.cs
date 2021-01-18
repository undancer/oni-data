using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(4524)]
	public struct HTML_ShowToolTip_t
	{
		public const int k_iCallback = 4524;

		public HHTMLBrowser unBrowserHandle;

		public string pchMsg;
	}
}
