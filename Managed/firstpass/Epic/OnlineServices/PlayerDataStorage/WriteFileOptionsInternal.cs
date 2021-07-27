using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.PlayerDataStorage
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct WriteFileOptionsInternal : IDisposable, IInitializable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Filename;

		private uint m_ChunkLengthBytes;

		private OnWriteFileDataCallbackInternal m_WriteFileDataCallback;

		private OnFileTransferProgressCallbackInternal m_FileTransferProgressCallback;

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

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
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

		public uint ChunkLengthBytes
		{
			get
			{
				uint target = Helper.GetDefault<uint>();
				Helper.TryMarshalGet(m_ChunkLengthBytes, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ChunkLengthBytes, value);
			}
		}

		public OnWriteFileDataCallbackInternal WriteFileDataCallback => m_WriteFileDataCallback;

		public OnFileTransferProgressCallbackInternal FileTransferProgressCallback => m_FileTransferProgressCallback;

		public void Initialize()
		{
			m_WriteFileDataCallback = PlayerDataStorageInterface.OnWriteFileData;
			m_FileTransferProgressCallback = PlayerDataStorageInterface.OnFileTransferProgress;
		}

		public void Dispose()
		{
		}
	}
}
