using System;

namespace rail
{
	public class IRailLeaderboardImpl : RailObject, IRailLeaderboard, IRailComponent
	{
		internal IRailLeaderboardImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailLeaderboardImpl()
		{
		}

		public virtual string GetLeaderboardName()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailLeaderboard_GetLeaderboardName(swigCPtr_));
		}

		public virtual string GetLeaderboardDisplayName()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailLeaderboard_GetLeaderboardDisplayName(swigCPtr_));
		}

		public virtual int GetTotalEntriesCount()
		{
			return RAIL_API_PINVOKE.IRailLeaderboard_GetTotalEntriesCount(swigCPtr_);
		}

		public virtual RailResult AsyncGetLeaderboard(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailLeaderboard_AsyncGetLeaderboard(swigCPtr_, user_data);
		}

		public virtual RailResult GetLeaderboardParameters(LeaderboardParameters param)
		{
			IntPtr intPtr = ((param == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_LeaderboardParameters__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailLeaderboard_GetLeaderboardParameters(swigCPtr_, intPtr);
			}
			finally
			{
				if (param != null)
				{
					RailConverter.Cpp2Csharp(intPtr, param);
				}
				RAIL_API_PINVOKE.delete_LeaderboardParameters(intPtr);
			}
		}

		public virtual IRailLeaderboardEntries CreateLeaderboardEntries()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailLeaderboard_CreateLeaderboardEntries(swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailLeaderboardEntriesImpl(intPtr);
			}
			return null;
		}

		public virtual RailResult AsyncUploadLeaderboard(UploadLeaderboardParam update_param, string user_data)
		{
			IntPtr intPtr = ((update_param == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_UploadLeaderboardParam__SWIG_0());
			if (update_param != null)
			{
				RailConverter.Csharp2Cpp(update_param, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailLeaderboard_AsyncUploadLeaderboard(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_UploadLeaderboardParam(intPtr);
			}
		}

		public virtual RailResult GetLeaderboardSortType(out int sort_type)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailLeaderboard_GetLeaderboardSortType(swigCPtr_, out sort_type);
		}

		public virtual RailResult GetLeaderboardDisplayType(out int display_type)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailLeaderboard_GetLeaderboardDisplayType(swigCPtr_, out display_type);
		}

		public virtual RailResult AsyncAttachSpaceWork(SpaceWorkID spacework_id, string user_data)
		{
			IntPtr intPtr = ((spacework_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (spacework_id != null)
			{
				RailConverter.Csharp2Cpp(spacework_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailLeaderboard_AsyncAttachSpaceWork(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
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
