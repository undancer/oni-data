using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(332)]
	public struct GameServerChangeRequested_t
	{
		public const int k_iCallback = 332;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private byte[] m_rgchServer_;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private byte[] m_rgchPassword_;

		public string m_rgchServer
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchServer_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchServer_, 64);
			}
		}

		public string m_rgchPassword
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchPassword_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchPassword_, 64);
			}
		}
	}
}
