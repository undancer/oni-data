using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.PlayerDataStorage
{
	public sealed class PlayerDataStorageInterface : Handle
	{
		public const int WritefileoptionsApiLatest = 1;

		public const int ReadfileoptionsApiLatest = 1;

		public const int DeletefileoptionsApiLatest = 1;

		public const int DuplicatefileoptionsApiLatest = 1;

		public const int CopyfilemetadatabyfilenameoptionsApiLatest = 1;

		public const int CopyfilemetadataatindexoptionsApiLatest = 1;

		public const int GetfilemetadatacountoptionsApiLatest = 1;

		public const int QueryfilelistoptionsApiLatest = 1;

		public const int QueryfileoptionsApiLatest = 1;

		public const int FilemetadataApiLatest = 1;

		public const int FileMaxSizeBytes = 67108864;

		public const int FilenameMaxLengthBytes = 64;

		public PlayerDataStorageInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryFile(QueryFileOptions queryFileOptions, object clientData, OnQueryFileCompleteCallback completionCallback)
		{
			QueryFileOptionsInternal queryFileOptions2 = Helper.CopyProperties<QueryFileOptionsInternal>(queryFileOptions);
			OnQueryFileCompleteCallbackInternal onQueryFileCompleteCallbackInternal = OnQueryFileComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onQueryFileCompleteCallbackInternal);
			EOS_PlayerDataStorage_QueryFile(base.InnerHandle, ref queryFileOptions2, clientDataAddress, onQueryFileCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref queryFileOptions2);
		}

		public void QueryFileList(QueryFileListOptions queryFileListOptions, object clientData, OnQueryFileListCompleteCallback completionCallback)
		{
			QueryFileListOptionsInternal queryFileListOptions2 = Helper.CopyProperties<QueryFileListOptionsInternal>(queryFileListOptions);
			OnQueryFileListCompleteCallbackInternal onQueryFileListCompleteCallbackInternal = OnQueryFileListComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onQueryFileListCompleteCallbackInternal);
			EOS_PlayerDataStorage_QueryFileList(base.InnerHandle, ref queryFileListOptions2, clientDataAddress, onQueryFileListCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref queryFileListOptions2);
		}

		public Result CopyFileMetadataByFilename(CopyFileMetadataByFilenameOptions copyFileMetadataOptions, out FileMetadata outMetadata)
		{
			CopyFileMetadataByFilenameOptionsInternal copyFileMetadataOptions2 = Helper.CopyProperties<CopyFileMetadataByFilenameOptionsInternal>(copyFileMetadataOptions);
			outMetadata = Helper.GetDefault<FileMetadata>();
			IntPtr outMetadata2 = IntPtr.Zero;
			Result source = EOS_PlayerDataStorage_CopyFileMetadataByFilename(base.InnerHandle, ref copyFileMetadataOptions2, ref outMetadata2);
			Helper.TryMarshalDispose(ref copyFileMetadataOptions2);
			if (Helper.TryMarshalGet<FileMetadataInternal, FileMetadata>(outMetadata2, out outMetadata))
			{
				EOS_PlayerDataStorage_FileMetadata_Release(outMetadata2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result GetFileMetadataCount(GetFileMetadataCountOptions getFileMetadataCountOptions, out int outFileMetadataCount)
		{
			GetFileMetadataCountOptionsInternal getFileMetadataCountOptions2 = Helper.CopyProperties<GetFileMetadataCountOptionsInternal>(getFileMetadataCountOptions);
			outFileMetadataCount = Helper.GetDefault<int>();
			Result source = EOS_PlayerDataStorage_GetFileMetadataCount(base.InnerHandle, ref getFileMetadataCountOptions2, ref outFileMetadataCount);
			Helper.TryMarshalDispose(ref getFileMetadataCountOptions2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyFileMetadataAtIndex(CopyFileMetadataAtIndexOptions copyFileMetadataOptions, out FileMetadata outMetadata)
		{
			CopyFileMetadataAtIndexOptionsInternal copyFileMetadataOptions2 = Helper.CopyProperties<CopyFileMetadataAtIndexOptionsInternal>(copyFileMetadataOptions);
			outMetadata = Helper.GetDefault<FileMetadata>();
			IntPtr outMetadata2 = IntPtr.Zero;
			Result source = EOS_PlayerDataStorage_CopyFileMetadataAtIndex(base.InnerHandle, ref copyFileMetadataOptions2, ref outMetadata2);
			Helper.TryMarshalDispose(ref copyFileMetadataOptions2);
			if (Helper.TryMarshalGet<FileMetadataInternal, FileMetadata>(outMetadata2, out outMetadata))
			{
				EOS_PlayerDataStorage_FileMetadata_Release(outMetadata2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public void DuplicateFile(DuplicateFileOptions duplicateOptions, object clientData, OnDuplicateFileCompleteCallback completionCallback)
		{
			DuplicateFileOptionsInternal duplicateOptions2 = Helper.CopyProperties<DuplicateFileOptionsInternal>(duplicateOptions);
			OnDuplicateFileCompleteCallbackInternal onDuplicateFileCompleteCallbackInternal = OnDuplicateFileComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onDuplicateFileCompleteCallbackInternal);
			EOS_PlayerDataStorage_DuplicateFile(base.InnerHandle, ref duplicateOptions2, clientDataAddress, onDuplicateFileCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref duplicateOptions2);
		}

		public void DeleteFile(DeleteFileOptions deleteOptions, object clientData, OnDeleteFileCompleteCallback completionCallback)
		{
			DeleteFileOptionsInternal deleteOptions2 = Helper.CopyProperties<DeleteFileOptionsInternal>(deleteOptions);
			OnDeleteFileCompleteCallbackInternal onDeleteFileCompleteCallbackInternal = OnDeleteFileComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onDeleteFileCompleteCallbackInternal);
			EOS_PlayerDataStorage_DeleteFile(base.InnerHandle, ref deleteOptions2, clientDataAddress, onDeleteFileCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref deleteOptions2);
		}

		public PlayerDataStorageFileTransferRequest ReadFile(ReadFileOptions readOptions, object clientData, OnReadFileCompleteCallback completionCallback)
		{
			ReadFileOptionsInternal readOptions2 = Helper.CopyProperties<ReadFileOptionsInternal>(readOptions);
			OnReadFileCompleteCallbackInternal onReadFileCompleteCallbackInternal = OnReadFileComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onReadFileCompleteCallbackInternal, readOptions.ReadFileDataCallback, readOptions2.ReadFileDataCallback, readOptions.FileTransferProgressCallback, readOptions2.FileTransferProgressCallback);
			IntPtr source = EOS_PlayerDataStorage_ReadFile(base.InnerHandle, ref readOptions2, clientDataAddress, onReadFileCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref readOptions2);
			PlayerDataStorageFileTransferRequest target = Helper.GetDefault<PlayerDataStorageFileTransferRequest>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public PlayerDataStorageFileTransferRequest WriteFile(WriteFileOptions writeOptions, object clientData, OnWriteFileCompleteCallback completionCallback)
		{
			WriteFileOptionsInternal writeOptions2 = Helper.CopyProperties<WriteFileOptionsInternal>(writeOptions);
			OnWriteFileCompleteCallbackInternal onWriteFileCompleteCallbackInternal = OnWriteFileComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onWriteFileCompleteCallbackInternal, writeOptions.WriteFileDataCallback, writeOptions2.WriteFileDataCallback, writeOptions.FileTransferProgressCallback, writeOptions2.FileTransferProgressCallback);
			IntPtr source = EOS_PlayerDataStorage_WriteFile(base.InnerHandle, ref writeOptions2, clientDataAddress, onWriteFileCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref writeOptions2);
			PlayerDataStorageFileTransferRequest target = Helper.GetDefault<PlayerDataStorageFileTransferRequest>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		[MonoPInvokeCallback]
		internal static WriteResult OnWriteFileData(IntPtr callbackInfoAddress, IntPtr outDataBuffer, ref uint outDataWritten)
		{
			OnWriteFileDataCallback callback = null;
			WriteFileDataCallbackInfo callbackInfo = null;
			if (Helper.TryGetAdditionalCallback<OnWriteFileDataCallback, WriteFileDataCallbackInfoInternal, WriteFileDataCallbackInfo>(callbackInfoAddress, out callback, out callbackInfo))
			{
				byte[] outDataBuffer2 = null;
				WriteResult result = callback(callbackInfo, out outDataBuffer2, out outDataWritten);
				Marshal.Copy(outDataBuffer2, 0, outDataBuffer, (int)outDataWritten);
				return result;
			}
			return Helper.GetDefault<WriteResult>();
		}

		[MonoPInvokeCallback]
		internal static void OnFileTransferProgress(IntPtr callbackInfoAddress)
		{
			OnFileTransferProgressCallback callback = null;
			FileTransferProgressCallbackInfo callbackInfo = null;
			if (Helper.TryGetAdditionalCallback<OnFileTransferProgressCallback, FileTransferProgressCallbackInfoInternal, FileTransferProgressCallbackInfo>(callbackInfoAddress, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static ReadResult OnReadFileData(IntPtr callbackInfoAddress)
		{
			OnReadFileDataCallback callback = null;
			ReadFileDataCallbackInfo callbackInfo = null;
			if (Helper.TryGetAdditionalCallback<OnReadFileDataCallback, ReadFileDataCallbackInfoInternal, ReadFileDataCallbackInfo>(callbackInfoAddress, out callback, out callbackInfo))
			{
				return callback(callbackInfo);
			}
			return Helper.GetDefault<ReadResult>();
		}

		[MonoPInvokeCallback]
		internal static void OnWriteFileComplete(IntPtr address)
		{
			OnWriteFileCompleteCallback callback = null;
			WriteFileCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnWriteFileCompleteCallback, WriteFileCallbackInfoInternal, WriteFileCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnReadFileComplete(IntPtr address)
		{
			OnReadFileCompleteCallback callback = null;
			ReadFileCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnReadFileCompleteCallback, ReadFileCallbackInfoInternal, ReadFileCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDeleteFileComplete(IntPtr address)
		{
			OnDeleteFileCompleteCallback callback = null;
			DeleteFileCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDeleteFileCompleteCallback, DeleteFileCallbackInfoInternal, DeleteFileCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnDuplicateFileComplete(IntPtr address)
		{
			OnDuplicateFileCompleteCallback callback = null;
			DuplicateFileCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDuplicateFileCompleteCallback, DuplicateFileCallbackInfoInternal, DuplicateFileCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryFileListComplete(IntPtr address)
		{
			OnQueryFileListCompleteCallback callback = null;
			QueryFileListCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryFileListCompleteCallback, QueryFileListCallbackInfoInternal, QueryFileListCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryFileComplete(IntPtr address)
		{
			OnQueryFileCompleteCallback callback = null;
			QueryFileCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryFileCompleteCallback, QueryFileCallbackInfoInternal, QueryFileCallbackInfo>(address, out callback, out callbackInfo))
			{
				callback(callbackInfo);
			}
		}

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_PlayerDataStorage_FileMetadata_Release(IntPtr fileMetadata);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_PlayerDataStorage_WriteFile(IntPtr handle, ref WriteFileOptionsInternal writeOptions, IntPtr clientData, OnWriteFileCompleteCallbackInternal completionCallback);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_PlayerDataStorage_ReadFile(IntPtr handle, ref ReadFileOptionsInternal readOptions, IntPtr clientData, OnReadFileCompleteCallbackInternal completionCallback);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_PlayerDataStorage_DeleteFile(IntPtr handle, ref DeleteFileOptionsInternal deleteOptions, IntPtr clientData, OnDeleteFileCompleteCallbackInternal completionCallback);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_PlayerDataStorage_DuplicateFile(IntPtr handle, ref DuplicateFileOptionsInternal duplicateOptions, IntPtr clientData, OnDuplicateFileCompleteCallbackInternal completionCallback);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_PlayerDataStorage_CopyFileMetadataAtIndex(IntPtr handle, ref CopyFileMetadataAtIndexOptionsInternal copyFileMetadataOptions, ref IntPtr outMetadata);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_PlayerDataStorage_GetFileMetadataCount(IntPtr handle, ref GetFileMetadataCountOptionsInternal getFileMetadataCountOptions, ref int outFileMetadataCount);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_PlayerDataStorage_CopyFileMetadataByFilename(IntPtr handle, ref CopyFileMetadataByFilenameOptionsInternal copyFileMetadataOptions, ref IntPtr outMetadata);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_PlayerDataStorage_QueryFileList(IntPtr handle, ref QueryFileListOptionsInternal queryFileListOptions, IntPtr clientData, OnQueryFileListCompleteCallbackInternal completionCallback);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_PlayerDataStorage_QueryFile(IntPtr handle, ref QueryFileOptionsInternal queryFileOptions, IntPtr clientData, OnQueryFileCompleteCallbackInternal completionCallback);
	}
}
