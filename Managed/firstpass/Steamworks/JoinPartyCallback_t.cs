using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(5301)]
	public struct JoinPartyCallback_t
	{
		public const int k_iCallback = 5301;

		public EResult m_eResult;

		public PartyBeaconID_t m_ulBeaconID;

		public CSteamID m_SteamIDBeaconOwner;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_rgchConnectString_;

		public string m_rgchConnectString
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchConnectString_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchConnectString_, 256);
			}
		}
	}
}
