using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.TitleStorage
{
	public sealed class TitleStorageInterface : Handle
	{
		public const int DeletecacheoptionsApiLatest = 1;

		public const int ReadfileoptionsApiLatest = 1;

		public const int CopyfilemetadatabyfilenameoptionsApiLatest = 1;

		public const int CopyfilemetadataatindexoptionsApiLatest = 1;

		public const int GetfilemetadatacountoptionsApiLatest = 1;

		public const int QueryfilelistoptionsApiLatest = 1;

		public const int QueryfileoptionsApiLatest = 1;

		public const int FilemetadataApiLatest = 1;

		public const int FilenameMaxLengthBytes = 64;

		public TitleStorageInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryFile(QueryFileOptions options, object clientData, OnQueryFileCompleteCallback completionCallback)
		{
			QueryFileOptionsInternal options2 = Helper.CopyProperties<QueryFileOptionsInternal>(options);
			OnQueryFileCompleteCallbackInternal onQueryFileCompleteCallbackInternal = OnQueryFileComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onQueryFileCompleteCallbackInternal);
			EOS_TitleStorage_QueryFile(base.InnerHandle, ref options2, clientDataAddress, onQueryFileCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public void QueryFileList(QueryFileListOptions options, object clientData, OnQueryFileListCompleteCallback completionCallback)
		{
			QueryFileListOptionsInternal options2 = Helper.CopyProperties<QueryFileListOptionsInternal>(options);
			OnQueryFileListCompleteCallbackInternal onQueryFileListCompleteCallbackInternal = OnQueryFileListComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onQueryFileListCompleteCallbackInternal);
			EOS_TitleStorage_QueryFileList(base.InnerHandle, ref options2, clientDataAddress, onQueryFileListCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
		}

		public Result CopyFileMetadataByFilename(CopyFileMetadataByFilenameOptions options, out FileMetadata outMetadata)
		{
			CopyFileMetadataByFilenameOptionsInternal options2 = Helper.CopyProperties<CopyFileMetadataByFilenameOptionsInternal>(options);
			outMetadata = Helper.GetDefault<FileMetadata>();
			IntPtr outMetadata2 = IntPtr.Zero;
			Result source = EOS_TitleStorage_CopyFileMetadataByFilename(base.InnerHandle, ref options2, ref outMetadata2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<FileMetadataInternal, FileMetadata>(outMetadata2, out outMetadata))
			{
				EOS_TitleStorage_FileMetadata_Release(outMetadata2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public uint GetFileMetadataCount(GetFileMetadataCountOptions options)
		{
			GetFileMetadataCountOptionsInternal options2 = Helper.CopyProperties<GetFileMetadataCountOptionsInternal>(options);
			uint source = EOS_TitleStorage_GetFileMetadataCount(base.InnerHandle, ref options2);
			Helper.TryMarshalDispose(ref options2);
			uint target = Helper.GetDefault<uint>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result CopyFileMetadataAtIndex(CopyFileMetadataAtIndexOptions options, out FileMetadata outMetadata)
		{
			CopyFileMetadataAtIndexOptionsInternal options2 = Helper.CopyProperties<CopyFileMetadataAtIndexOptionsInternal>(options);
			outMetadata = Helper.GetDefault<FileMetadata>();
			IntPtr outMetadata2 = IntPtr.Zero;
			Result source = EOS_TitleStorage_CopyFileMetadataAtIndex(base.InnerHandle, ref options2, ref outMetadata2);
			Helper.TryMarshalDispose(ref options2);
			if (Helper.TryMarshalGet<FileMetadataInternal, FileMetadata>(outMetadata2, out outMetadata))
			{
				EOS_TitleStorage_FileMetadata_Release(outMetadata2);
			}
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public TitleStorageFileTransferRequest ReadFile(ReadFileOptions options, object clientData, OnReadFileCompleteCallback completionCallback)
		{
			ReadFileOptionsInternal options2 = Helper.CopyProperties<ReadFileOptionsInternal>(options);
			OnReadFileCompleteCallbackInternal onReadFileCompleteCallbackInternal = OnReadFileComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onReadFileCompleteCallbackInternal, options.ReadFileDataCallback, options2.ReadFileDataCallback, options.FileTransferProgressCallback, options2.FileTransferProgressCallback);
			IntPtr source = EOS_TitleStorage_ReadFile(base.InnerHandle, ref options2, clientDataAddress, onReadFileCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			TitleStorageFileTransferRequest target = Helper.GetDefault<TitleStorageFileTransferRequest>();
			Helper.TryMarshalGet(source, out target);
			return target;
		}

		public Result DeleteCache(DeleteCacheOptions options, object clientData, OnDeleteCacheCompleteCallback completionCallback)
		{
			DeleteCacheOptionsInternal options2 = Helper.CopyProperties<DeleteCacheOptionsInternal>(options);
			OnDeleteCacheCompleteCallbackInternal onDeleteCacheCompleteCallbackInternal = OnDeleteCacheComplete;
			IntPtr clientDataAddress = IntPtr.Zero;
			Helper.AddCallback(ref clientDataAddress, clientData, completionCallback, onDeleteCacheCompleteCallbackInternal);
			Result source = EOS_TitleStorage_DeleteCache(base.InnerHandle, ref options2, clientDataAddress, onDeleteCacheCompleteCallbackInternal);
			Helper.TryMarshalDispose(ref options2);
			Result target = Helper.GetDefault<Result>();
			Helper.TryMarshalGet(source, out target);
			return target;
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
		internal static void OnDeleteCacheComplete(IntPtr address)
		{
			OnDeleteCacheCompleteCallback callback = null;
			DeleteCacheCallbackInfo callbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnDeleteCacheCompleteCallback, DeleteCacheCallbackInfoInternal, DeleteCacheCallbackInfo>(address, out callback, out callbackInfo))
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
		private static extern void EOS_TitleStorage_FileMetadata_Release(IntPtr fileMetadata);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_TitleStorage_DeleteCache(IntPtr handle, ref DeleteCacheOptionsInternal options, IntPtr clientData, OnDeleteCacheCompleteCallbackInternal completionCallback);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern IntPtr EOS_TitleStorage_ReadFile(IntPtr handle, ref ReadFileOptionsInternal options, IntPtr clientData, OnReadFileCompleteCallbackInternal completionCallback);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_TitleStorage_CopyFileMetadataAtIndex(IntPtr handle, ref CopyFileMetadataAtIndexOptionsInternal options, ref IntPtr outMetadata);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern uint EOS_TitleStorage_GetFileMetadataCount(IntPtr handle, ref GetFileMetadataCountOptionsInternal options);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern Result EOS_TitleStorage_CopyFileMetadataByFilename(IntPtr handle, ref CopyFileMetadataByFilenameOptionsInternal options, ref IntPtr outMetadata);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_TitleStorage_QueryFileList(IntPtr handle, ref QueryFileListOptionsInternal options, IntPtr clientData, OnQueryFileListCompleteCallbackInternal completionCallback);

		[DllImport("libEOSSDK-Mac-Shipping")]
		private static extern void EOS_TitleStorage_QueryFile(IntPtr handle, ref QueryFileOptionsInternal options, IntPtr clientData, OnQueryFileCompleteCallbackInternal completionCallback);
	}
}
