using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailStorageHelperImpl : RailObject, IRailStorageHelper
	{
		internal IRailStorageHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailStorageHelperImpl()
		{
		}

		public virtual IRailFile OpenFile(string filename, out RailResult result)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStorageHelper_OpenFile__SWIG_0(swigCPtr_, filename, out result);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFileImpl(intPtr);
			}
			return null;
		}

		public virtual IRailFile OpenFile(string filename)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStorageHelper_OpenFile__SWIG_1(swigCPtr_, filename);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFileImpl(intPtr);
			}
			return null;
		}

		public virtual IRailFile CreateFile(string filename, out RailResult result)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStorageHelper_CreateFile__SWIG_0(swigCPtr_, filename, out result);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFileImpl(intPtr);
			}
			return null;
		}

		public virtual IRailFile CreateFile(string filename)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStorageHelper_CreateFile__SWIG_1(swigCPtr_, filename);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFileImpl(intPtr);
			}
			return null;
		}

		public virtual bool IsFileExist(string filename)
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_IsFileExist(swigCPtr_, filename);
		}

		public virtual bool ListFiles(List<string> filelist)
		{
			IntPtr intPtr = ((filelist == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			try
			{
				return RAIL_API_PINVOKE.IRailStorageHelper_ListFiles(swigCPtr_, intPtr);
			}
			finally
			{
				if (filelist != null)
				{
					RailConverter.Cpp2Csharp(intPtr, filelist);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult RemoveFile(string filename)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_RemoveFile(swigCPtr_, filename);
		}

		public virtual bool IsFileSyncedToCloud(string filename)
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_IsFileSyncedToCloud(swigCPtr_, filename);
		}

		public virtual RailResult GetFileTimestamp(string filename, out ulong time_stamp)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_GetFileTimestamp(swigCPtr_, filename, out time_stamp);
		}

		public virtual uint GetFileCount()
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_GetFileCount(swigCPtr_);
		}

		public virtual RailResult GetFileNameAndSize(uint file_index, out string filename, out ulong file_size)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_GetFileNameAndSize(swigCPtr_, file_index, intPtr, out file_size);
			}
			finally
			{
				filename = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult AsyncQueryQuota()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncQueryQuota(swigCPtr_);
		}

		public virtual RailResult SetSyncFileOption(string filename, RailSyncFileOption option)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSyncFileOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_SetSyncFileOption(swigCPtr_, filename, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSyncFileOption(intPtr);
			}
		}

		public virtual bool IsCloudStorageEnabledForApp()
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_IsCloudStorageEnabledForApp(swigCPtr_);
		}

		public virtual bool IsCloudStorageEnabledForPlayer()
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_IsCloudStorageEnabledForPlayer(swigCPtr_);
		}

		public virtual RailResult AsyncPublishFileToUserSpace(RailPublishFileToUserSpaceOption option, string user_data)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailPublishFileToUserSpaceOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncPublishFileToUserSpace(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailPublishFileToUserSpaceOption(intPtr);
			}
		}

		public virtual IRailStreamFile OpenStreamFile(string filename, RailStreamFileOption option, out RailResult result)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailStreamFileOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailStorageHelper_OpenStreamFile__SWIG_0(swigCPtr_, filename, intPtr, out result);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailStreamFileImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailStreamFileOption(intPtr);
			}
		}

		public virtual IRailStreamFile OpenStreamFile(string filename, RailStreamFileOption option)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailStreamFileOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailStorageHelper_OpenStreamFile__SWIG_1(swigCPtr_, filename, intPtr);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailStreamFileImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailStreamFileOption(intPtr);
			}
		}

		public virtual RailResult AsyncListStreamFiles(string contents, RailListStreamFileOption option, string user_data)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailListStreamFileOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncListStreamFiles(swigCPtr_, contents, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailListStreamFileOption(intPtr);
			}
		}

		public virtual RailResult AsyncRenameStreamFile(string old_filename, string new_filename, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncRenameStreamFile(swigCPtr_, old_filename, new_filename, user_data);
		}

		public virtual RailResult AsyncDeleteStreamFile(string filename, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncDeleteStreamFile(swigCPtr_, filename, user_data);
		}

		public virtual uint GetRailFileEnabledOS(string filename)
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_GetRailFileEnabledOS(swigCPtr_, filename);
		}

		public virtual RailResult SetRailFileEnabledOS(string filename, EnumRailStorageFileEnabledOS sync_os)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_SetRailFileEnabledOS(swigCPtr_, filename, (int)sync_os);
		}
	}
}
