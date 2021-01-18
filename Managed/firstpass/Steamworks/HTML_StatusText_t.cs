using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(4523)]
	public struct HTML_StatusText_t
	{
		public const int k_iCallback = 4523;

		public HHTMLBrowser unBrowserHandle;

		public string pchMsg;
	}
}
