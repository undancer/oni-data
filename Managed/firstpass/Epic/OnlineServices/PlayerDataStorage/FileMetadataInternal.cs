using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.PlayerDataStorage
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct FileMetadataInternal : IDisposable
	{
		private int m_ApiVersion;

		private uint m_FileSizeBytes;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_MD5Hash;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Filename;

		public int ApiVersion
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ApiVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ApiVersion, value);
			}
		}

		public uint FileSizeBytes
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_FileSizeBytes, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_FileSizeBytes, value);
			}
		}

		public string MD5Hash
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_MD5Hash, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_MD5Hash, value);
			}
		}

		public string Filename
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Filename, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Filename, value);
			}
		}

		public void Dispose()
		{
		}
	}
}
