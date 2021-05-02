using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(202)]
	public struct GSClientDeny_t
	{
		public const int k_iCallback = 202;

		public CSteamID m_SteamID;

		public EDenyReason m_eDenyReason;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private byte[] m_rgchOptionalText_;

		public string m_rgchOptionalText
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchOptionalText_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchOptionalText_, 128);
			}
		}
	}
}
