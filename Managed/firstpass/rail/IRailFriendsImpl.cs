using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailFriendsImpl : RailObject, IRailFriends
	{
		internal IRailFriendsImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailFriendsImpl()
		{
		}

		public virtual RailResult AsyncGetPersonalInfo(List<RailID> rail_ids, string user_data)
		{
			IntPtr intPtr = ((rail_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (rail_ids != null)
			{
				RailConverter.Csharp2Cpp(rail_ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncGetPersonalInfo(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual RailResult AsyncGetFriendMetadata(RailID rail_id, List<string> keys, string user_data)
		{
			IntPtr intPtr = ((rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (rail_id != null)
			{
				RailConverter.Csharp2Cpp(rail_id, intPtr);
			}
			IntPtr intPtr2 = ((keys == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (keys != null)
			{
				RailConverter.Csharp2Cpp(keys, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncGetFriendMetadata(swigCPtr_, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr2);
			}
		}

		public virtual RailResult AsyncSetMyMetadata(List<RailKeyValue> key_values, string user_data)
		{
			IntPtr intPtr = ((key_values == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			if (key_values != null)
			{
				RailConverter.Csharp2Cpp(key_values, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncSetMyMetadata(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
		}

		public virtual RailResult AsyncClearAllMyMetadata(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncClearAllMyMetadata(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncSetInviteCommandLine(string command_line, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncSetInviteCommandLine(swigCPtr_, command_line, user_data);
		}

		public virtual RailResult AsyncGetInviteCommandLine(RailID rail_id, string user_data)
		{
			IntPtr intPtr = ((rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (rail_id != null)
			{
				RailConverter.Csharp2Cpp(rail_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncGetInviteCommandLine(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncReportPlayedWithUserList(List<RailUserPlayedWith> player_list, string user_data)
		{
			IntPtr intPtr = ((player_list == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailUserPlayedWith__SWIG_0());
			if (player_list != null)
			{
				RailConverter.Csharp2Cpp(player_list, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncReportPlayedWithUserList(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailUserPlayedWith(intPtr);
			}
		}

		public virtual RailResult GetFriendsList(List<RailFriendInfo> friends_list)
		{
			IntPtr intPtr = ((friends_list == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailFriendInfo__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_GetFriendsList(swigCPtr_, intPtr);
			}
			finally
			{
				if (friends_list != null)
				{
					RailConverter.Cpp2Csharp(intPtr, friends_list);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailFriendInfo(intPtr);
			}
		}

		public virtual RailResult AsyncQueryFriendPlayedGamesInfo(RailID rail_id, string user_data)
		{
			IntPtr intPtr = ((rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (rail_id != null)
			{
				RailConverter.Csharp2Cpp(rail_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncQueryFriendPlayedGamesInfo(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncQueryPlayedWithFriendsList(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncQueryPlayedWithFriendsList(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncQueryPlayedWithFriendsTime(List<RailID> rail_ids, string user_data)
		{
			IntPtr intPtr = ((rail_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (rail_ids != null)
			{
				RailConverter.Csharp2Cpp(rail_ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncQueryPlayedWithFriendsTime(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual RailResult AsyncQueryPlayedWithFriendsGames(List<RailID> rail_ids, string user_data)
		{
			IntPtr intPtr = ((rail_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (rail_ids != null)
			{
				RailConverter.Csharp2Cpp(rail_ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncQueryPlayedWithFriendsGames(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual RailResult AsyncAddFriend(RailFriendsAddFriendRequest request, string user_data)
		{
			IntPtr intPtr = ((request == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailFriendsAddFriendRequest__SWIG_0());
			if (request != null)
			{
				RailConverter.Csharp2Cpp(request, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncAddFriend(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailFriendsAddFriendRequest(intPtr);
			}
		}

		public virtual RailResult AsyncUpdateFriendsData(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFriends_AsyncUpdateFriendsData(swigCPtr_, user_data);
		}
	}
}
