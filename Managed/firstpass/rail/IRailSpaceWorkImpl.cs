using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailSpaceWorkImpl : RailObject, IRailSpaceWork, IRailComponent
	{
		internal IRailSpaceWorkImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailSpaceWorkImpl()
		{
		}

		public virtual void Close()
		{
			RAIL_API_PINVOKE.IRailSpaceWork_Close(swigCPtr_);
		}

		public virtual SpaceWorkID GetSpaceWorkID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailSpaceWork_GetSpaceWorkID(swigCPtr_);
			SpaceWorkID spaceWorkID = new SpaceWorkID();
			RailConverter.Cpp2Csharp(ptr, spaceWorkID);
			return spaceWorkID;
		}

		public virtual bool Editable()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_Editable(swigCPtr_);
		}

		public virtual RailResult StartSync(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_StartSync(swigCPtr_, user_data);
		}

		public virtual RailResult GetSyncProgress(RailSpaceWorkSyncProgress progress)
		{
			IntPtr intPtr = ((progress == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkSyncProgress__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetSyncProgress(swigCPtr_, intPtr);
			}
			finally
			{
				if (progress != null)
				{
					RailConverter.Cpp2Csharp(intPtr, progress);
				}
				RAIL_API_PINVOKE.delete_RailSpaceWorkSyncProgress(intPtr);
			}
		}

		public virtual RailResult CancelSync()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_CancelSync(swigCPtr_);
		}

		public virtual RailResult GetWorkLocalFolder(out string path)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetWorkLocalFolder(swigCPtr_, intPtr);
			}
			finally
			{
				path = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult AsyncUpdateMetadata(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_AsyncUpdateMetadata(swigCPtr_, user_data);
		}

		public virtual RailResult GetName(out string name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetName(swigCPtr_, intPtr);
			}
			finally
			{
				name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult GetDescription(out string description)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetDescription(swigCPtr_, intPtr);
			}
			finally
			{
				description = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult GetUrl(out string url)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetUrl(swigCPtr_, intPtr);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual uint GetCreateTime()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetCreateTime(swigCPtr_);
		}

		public virtual uint GetLastUpdateTime()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetLastUpdateTime(swigCPtr_);
		}

		public virtual ulong GetWorkFileSize()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetWorkFileSize(swigCPtr_);
		}

		public virtual RailResult GetTags(List<string> tags)
		{
			IntPtr intPtr = ((tags == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetTags(swigCPtr_, intPtr);
			}
			finally
			{
				if (tags != null)
				{
					RailConverter.Cpp2Csharp(intPtr, tags);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult GetPreviewImage(out string path)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetPreviewImage(swigCPtr_, intPtr);
			}
			finally
			{
				path = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult GetVersion(out string version)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetVersion(swigCPtr_, intPtr);
			}
			finally
			{
				version = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual ulong GetDownloadCount()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetDownloadCount(swigCPtr_);
		}

		public virtual ulong GetSubscribedCount()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetSubscribedCount(swigCPtr_);
		}

		public virtual EnumRailSpaceWorkShareLevel GetShareLevel()
		{
			return (EnumRailSpaceWorkShareLevel)RAIL_API_PINVOKE.IRailSpaceWork_GetShareLevel(swigCPtr_);
		}

		public virtual ulong GetScore()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetScore(swigCPtr_);
		}

		public virtual RailResult GetMetadata(string key, out string value)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetMetadata(swigCPtr_, key, intPtr);
			}
			finally
			{
				value = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual EnumRailSpaceWorkRateValue GetMyVote()
		{
			return (EnumRailSpaceWorkRateValue)RAIL_API_PINVOKE.IRailSpaceWork_GetMyVote(swigCPtr_);
		}

		public virtual bool IsFavorite()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_IsFavorite(swigCPtr_);
		}

		public virtual bool IsSubscribed()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_IsSubscribed(swigCPtr_);
		}

		public virtual RailResult SetName(string name)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetName(swigCPtr_, name);
		}

		public virtual RailResult SetDescription(string description)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetDescription(swigCPtr_, description);
		}

		public virtual RailResult SetTags(List<string> tags)
		{
			IntPtr intPtr = ((tags == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (tags != null)
			{
				RailConverter.Csharp2Cpp(tags, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetTags(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult SetPreviewImage(string path_filename)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetPreviewImage(swigCPtr_, path_filename);
		}

		public virtual RailResult SetVersion(string version)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetVersion(swigCPtr_, version);
		}

		public virtual RailResult SetShareLevel(EnumRailSpaceWorkShareLevel level)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetShareLevel__SWIG_0(swigCPtr_, (int)level);
		}

		public virtual RailResult SetShareLevel()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetShareLevel__SWIG_1(swigCPtr_);
		}

		public virtual RailResult SetMetadata(string key, string value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetMetadata(swigCPtr_, key, value);
		}

		public virtual RailResult SetContentFromFolder(string path)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetContentFromFolder(swigCPtr_, path);
		}

		public virtual RailResult GetAllMetadata(List<RailKeyValue> metadata)
		{
			IntPtr intPtr = ((metadata == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetAllMetadata(swigCPtr_, intPtr);
			}
			finally
			{
				if (metadata != null)
				{
					RailConverter.Cpp2Csharp(intPtr, metadata);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
		}

		public virtual RailResult GetAdditionalPreviewUrls(List<string> preview_urls)
		{
			IntPtr intPtr = ((preview_urls == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetAdditionalPreviewUrls(swigCPtr_, intPtr);
			}
			finally
			{
				if (preview_urls != null)
				{
					RailConverter.Cpp2Csharp(intPtr, preview_urls);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult GetAssociatedSpaceWorks(List<SpaceWorkID> ids)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetAssociatedSpaceWorks(swigCPtr_, intPtr);
			}
			finally
			{
				if (ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, ids);
				}
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
		}

		public virtual RailResult GetLanguages(List<string> languages)
		{
			IntPtr intPtr = ((languages == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetLanguages(swigCPtr_, intPtr);
			}
			finally
			{
				if (languages != null)
				{
					RailConverter.Cpp2Csharp(intPtr, languages);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult RemoveMetadata(string key)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_RemoveMetadata(swigCPtr_, key);
		}

		public virtual RailResult SetAdditionalPreviews(List<string> local_paths)
		{
			IntPtr intPtr = ((local_paths == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (local_paths != null)
			{
				RailConverter.Csharp2Cpp(local_paths, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetAdditionalPreviews(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult SetAssociatedSpaceWorks(List<SpaceWorkID> ids)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (ids != null)
			{
				RailConverter.Csharp2Cpp(ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetAssociatedSpaceWorks(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
		}

		public virtual RailResult SetLanguages(List<string> languages)
		{
			IntPtr intPtr = ((languages == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (languages != null)
			{
				RailConverter.Csharp2Cpp(languages, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetLanguages(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult GetPreviewUrl(out string url, uint scaling)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetPreviewUrl__SWIG_0(swigCPtr_, intPtr, scaling);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult GetPreviewUrl(out string url)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetPreviewUrl__SWIG_1(swigCPtr_, intPtr);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult GetVoteDetail(List<RailSpaceWorkVoteDetail> vote_details)
		{
			IntPtr intPtr = ((vote_details == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailSpaceWorkVoteDetail__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetVoteDetail(swigCPtr_, intPtr);
			}
			finally
			{
				if (vote_details != null)
				{
					RailConverter.Cpp2Csharp(intPtr, vote_details);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailSpaceWorkVoteDetail(intPtr);
			}
		}

		public virtual RailResult GetUploaderIDs(List<RailID> uploader_ids)
		{
			IntPtr intPtr = ((uploader_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetUploaderIDs(swigCPtr_, intPtr);
			}
			finally
			{
				if (uploader_ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, uploader_ids);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual RailResult SetUpdateOptions(RailSpaceWorkUpdateOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkUpdateOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_SetUpdateOptions(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkUpdateOptions(intPtr);
			}
		}

		public virtual RailResult GetStatistic(EnumRailSpaceWorkStatistic stat_type, out ulong value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetStatistic(swigCPtr_, (int)stat_type, out value);
		}

		public virtual RailResult RemovePreviewImage()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_RemovePreviewImage(swigCPtr_);
		}

		public virtual uint GetState()
		{
			return RAIL_API_PINVOKE.IRailSpaceWork_GetState(swigCPtr_);
		}

		public virtual RailResult AddAssociatedGameIDs(List<RailGameID> game_ids)
		{
			IntPtr intPtr = ((game_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailGameID__SWIG_0());
			if (game_ids != null)
			{
				RailConverter.Csharp2Cpp(game_ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_AddAssociatedGameIDs(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailGameID(intPtr);
			}
		}

		public virtual RailResult RemoveAssociatedGameIDs(List<RailGameID> game_ids)
		{
			IntPtr intPtr = ((game_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailGameID__SWIG_0());
			if (game_ids != null)
			{
				RailConverter.Csharp2Cpp(game_ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_RemoveAssociatedGameIDs(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailGameID(intPtr);
			}
		}

		public virtual RailResult GetAssociatedGameIDs(List<RailGameID> game_ids)
		{
			IntPtr intPtr = ((game_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailGameID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetAssociatedGameIDs(swigCPtr_, intPtr);
			}
			finally
			{
				if (game_ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, game_ids);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailGameID(intPtr);
			}
		}

		public virtual RailResult GetLocalVersion(out string version)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSpaceWork_GetLocalVersion(swigCPtr_, intPtr);
			}
			finally
			{
				version = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual ulong GetComponentVersion()
		{
			return RAIL_API_PINVOKE.IRailComponent_GetComponentVersion(swigCPtr_);
		}

		public virtual void Release()
		{
			RAIL_API_PINVOKE.IRailComponent_Release(swigCPtr_);
		}
	}
}
