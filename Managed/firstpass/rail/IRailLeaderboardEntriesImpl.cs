using System;

namespace rail
{
	public class IRailLeaderboardEntriesImpl : RailObject, IRailLeaderboardEntries, IRailComponent
	{
		internal IRailLeaderboardEntriesImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailLeaderboardEntriesImpl()
		{
		}

		public virtual RailID GetRailID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailLeaderboardEntries_GetRailID(swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(ptr, railID);
			return railID;
		}

		public virtual string GetLeaderboardName()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailLeaderboardEntries_GetLeaderboardName(swigCPtr_));
		}

		public virtual RailResult AsyncRequestLeaderboardEntries(RailID player, RequestLeaderboardEntryParam param, string user_data)
		{
			IntPtr intPtr = ((player == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (player != null)
			{
				RailConverter.Csharp2Cpp(player, intPtr);
			}
			IntPtr intPtr2 = ((param == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RequestLeaderboardEntryParam__SWIG_0());
			if (param != null)
			{
				RailConverter.Csharp2Cpp(param, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailLeaderboardEntries_AsyncRequestLeaderboardEntries(swigCPtr_, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RequestLeaderboardEntryParam(intPtr2);
			}
		}

		public virtual RequestLeaderboardEntryParam GetEntriesParam()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailLeaderboardEntries_GetEntriesParam(swigCPtr_);
			RequestLeaderboardEntryParam requestLeaderboardEntryParam = new RequestLeaderboardEntryParam();
			RailConverter.Cpp2Csharp(ptr, requestLeaderboardEntryParam);
			return requestLeaderboardEntryParam;
		}

		public virtual int GetEntriesCount()
		{
			return RAIL_API_PINVOKE.IRailLeaderboardEntries_GetEntriesCount(swigCPtr_);
		}

		public virtual RailResult GetLeaderboardEntry(int index, LeaderboardEntry leaderboard_entry)
		{
			IntPtr intPtr = ((leaderboard_entry == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_LeaderboardEntry__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailLeaderboardEntries_GetLeaderboardEntry(swigCPtr_, index, intPtr);
			}
			finally
			{
				if (leaderboard_entry != null)
				{
					RailConverter.Cpp2Csharp(intPtr, leaderboard_entry);
				}
				RAIL_API_PINVOKE.delete_LeaderboardEntry(intPtr);
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
