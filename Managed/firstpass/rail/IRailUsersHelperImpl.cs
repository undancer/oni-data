using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailUsersHelperImpl : RailObject, IRailUsersHelper
	{
		internal IRailUsersHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailUsersHelperImpl()
		{
		}

		public virtual RailResult AsyncGetUsersInfo(List<RailID> rail_ids, string user_data)
		{
			IntPtr intPtr = ((rail_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (rail_ids != null)
			{
				RailConverter.Csharp2Cpp(rail_ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUsersHelper_AsyncGetUsersInfo(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual RailResult AsyncInviteUsers(string command_line, List<RailID> users, RailInviteOptions options, string user_data)
		{
			IntPtr intPtr = ((users == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (users != null)
			{
				RailConverter.Csharp2Cpp(users, intPtr);
			}
			IntPtr intPtr2 = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailInviteOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUsersHelper_AsyncInviteUsers(swigCPtr_, command_line, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
				RAIL_API_PINVOKE.delete_RailInviteOptions(intPtr2);
			}
		}

		public virtual RailResult AsyncGetInviteDetail(RailID inviter, EnumRailUsersInviteType invite_type, string user_data)
		{
			IntPtr intPtr = ((inviter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (inviter != null)
			{
				RailConverter.Csharp2Cpp(inviter, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUsersHelper_AsyncGetInviteDetail(swigCPtr_, intPtr, (int)invite_type, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncCancelInvite(EnumRailUsersInviteType invite_type, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailUsersHelper_AsyncCancelInvite(swigCPtr_, (int)invite_type, user_data);
		}

		public virtual RailResult AsyncCancelAllInvites(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailUsersHelper_AsyncCancelAllInvites(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncGetUserLimits(RailID user_id, string user_data)
		{
			IntPtr intPtr = ((user_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (user_id != null)
			{
				RailConverter.Csharp2Cpp(user_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUsersHelper_AsyncGetUserLimits(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncShowChatWindowWithFriend(RailID rail_id, string user_data)
		{
			IntPtr intPtr = ((rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (rail_id != null)
			{
				RailConverter.Csharp2Cpp(rail_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUsersHelper_AsyncShowChatWindowWithFriend(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncShowUserHomepageWindow(RailID rail_id, string user_data)
		{
			IntPtr intPtr = ((rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (rail_id != null)
			{
				RailConverter.Csharp2Cpp(rail_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUsersHelper_AsyncShowUserHomepageWindow(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}
	}
}
