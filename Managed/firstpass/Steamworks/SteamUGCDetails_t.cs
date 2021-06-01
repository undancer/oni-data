using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SteamUGCDetails_t
	{
		public PublishedFileId_t m_nPublishedFileId;

		public EResult m_eResult;

		public EWorkshopFileType m_eFileType;

		public AppId_t m_nCreatorAppID;

		public AppId_t m_nConsumerAppID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 129)]
		private byte[] m_rgchTitle_;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8000)]
		private byte[] m_rgchDescription_;

		public ulong m_ulSteamIDOwner;

		public uint m_rtimeCreated;

		public uint m_rtimeUpdated;

		public uint m_rtimeAddedToUserList;

		public ERemoteStoragePublishedFileVisibility m_eVisibility;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bBanned;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bAcceptedForUse;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bTagsTruncated;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025)]
		private byte[] m_rgchTags_;

		public UGCHandle_t m_hFile;

		public UGCHandle_t m_hPreviewFile;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
		private byte[] m_pchFileName_;

		public int m_nFileSize;

		public int m_nPreviewFileSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_rgchURL_;

		public uint m_unVotesUp;

		public uint m_unVotesDown;

		public float m_flScore;

		public uint m_unNumChildren;

		public string m_rgchTitle
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchTitle_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchTitle_, 129);
			}
		}

		public string m_rgchDescription
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchDescription_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchDescription_, 8000);
			}
		}

		public string m_rgchTags
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchTags_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchTags_, 1025);
			}
		}

		public string m_pchFileName
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_pchFileName_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_pchFileName_, 260);
			}
		}

		public string m_rgchURL
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(m_rgchURL_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, m_rgchURL_, 256);
			}
		}
	}
}
