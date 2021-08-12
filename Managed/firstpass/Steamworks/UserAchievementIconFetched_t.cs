using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[CallbackIdentity(1109)]
	public struct UserAchievementIconFetched_t
	{
		public const int k_iCallback = 1109;

		public CGameID m_nGameID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private byte[] m_rgchAchievementName_;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bAchieved;

		public int m_nIconHandle;

		public string m_rgchAchievementName
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchAchievementName_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchAchievementName_, 128);
			}
		}
	}
}
