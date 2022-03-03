using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailRoomHelperImpl : RailObject, IRailRoomHelper
	{
		internal IRailRoomHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailRoomHelperImpl()
		{
		}

		public virtual IRailRoom CreateRoom(RoomOptions options, string room_name, out RailResult result)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RoomOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailRoomHelper_CreateRoom(swigCPtr_, intPtr, room_name, out result);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailRoomImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RoomOptions(intPtr);
			}
		}

		public virtual IRailRoom AsyncCreateRoom(RoomOptions options, string room_name, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RoomOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailRoomHelper_AsyncCreateRoom(swigCPtr_, intPtr, room_name, user_data);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailRoomImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RoomOptions(intPtr);
			}
		}

		public virtual IRailRoom OpenRoom(ulong room_id, out RailResult result)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailRoomHelper_OpenRoom(swigCPtr_, room_id, out result);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailRoomImpl(intPtr);
			}
			return null;
		}

		public virtual IRailRoom AsyncOpenRoom(ulong room_id, string user_data)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailRoomHelper_AsyncOpenRoom(swigCPtr_, room_id, user_data);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailRoomImpl(intPtr);
			}
			return null;
		}

		public virtual RailResult AsyncGetRoomList(uint start_index, uint end_index, List<RoomInfoListSorter> sorter, List<RoomInfoListFilter> filter, string user_data)
		{
			IntPtr intPtr = ((sorter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRoomInfoListSorter__SWIG_0());
			if (sorter != null)
			{
				RailConverter.Csharp2Cpp(sorter, intPtr);
			}
			IntPtr intPtr2 = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRoomInfoListFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoomHelper_AsyncGetRoomList(swigCPtr_, start_index, end_index, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRoomInfoListSorter(intPtr);
				RAIL_API_PINVOKE.delete_RailArrayRoomInfoListFilter(intPtr2);
			}
		}

		public virtual RailResult AsyncGetRoomListByTags(uint start_index, uint end_index, List<RoomInfoListSorter> sorter, List<string> room_tags, string user_data)
		{
			IntPtr intPtr = ((sorter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRoomInfoListSorter__SWIG_0());
			if (sorter != null)
			{
				RailConverter.Csharp2Cpp(sorter, intPtr);
			}
			IntPtr intPtr2 = ((room_tags == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (room_tags != null)
			{
				RailConverter.Csharp2Cpp(room_tags, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoomHelper_AsyncGetRoomListByTags(swigCPtr_, start_index, end_index, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRoomInfoListSorter(intPtr);
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr2);
			}
		}

		public virtual RailResult AsyncGetUserRoomList(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoomHelper_AsyncGetUserRoomList(swigCPtr_, user_data);
		}
	}
}
