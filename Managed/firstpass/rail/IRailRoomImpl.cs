using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailRoomImpl : RailObject, IRailRoom, IRailComponent
	{
		internal IRailRoomImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailRoomImpl()
		{
		}

		public virtual RailResult AsyncJoinRoom(string password, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncJoinRoom(swigCPtr_, password, user_data);
		}

		public virtual ulong GetRoomID()
		{
			return RAIL_API_PINVOKE.IRailRoom_GetRoomID(swigCPtr_);
		}

		public virtual RailResult GetRoomName(out string name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_GetRoomName(swigCPtr_, intPtr);
			}
			finally
			{
				name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailID GetOwnerID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailRoom_GetOwnerID(swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(ptr, railID);
			return railID;
		}

		public virtual bool HasPassword()
		{
			return RAIL_API_PINVOKE.IRailRoom_HasPassword(swigCPtr_);
		}

		public virtual EnumRoomType GetRoomType()
		{
			return (EnumRoomType)RAIL_API_PINVOKE.IRailRoom_GetRoomType(swigCPtr_);
		}

		public virtual uint GetMembers()
		{
			return RAIL_API_PINVOKE.IRailRoom_GetMembers(swigCPtr_);
		}

		public virtual RailID GetMemberByIndex(uint index)
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailRoom_GetMemberByIndex(swigCPtr_, index);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(ptr, railID);
			return railID;
		}

		public virtual RailResult GetMemberNameByIndex(uint index, out string name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_GetMemberNameByIndex(swigCPtr_, index, intPtr);
			}
			finally
			{
				name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual uint GetMaxMembers()
		{
			return RAIL_API_PINVOKE.IRailRoom_GetMaxMembers(swigCPtr_);
		}

		public virtual void Leave()
		{
			RAIL_API_PINVOKE.IRailRoom_Leave(swigCPtr_);
		}

		public virtual RailResult AsyncSetNewRoomOwner(RailID new_owner_id, string user_data)
		{
			IntPtr intPtr = ((new_owner_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (new_owner_id != null)
			{
				RailConverter.Csharp2Cpp(new_owner_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncSetNewRoomOwner(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncGetRoomMembers(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncGetRoomMembers(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncGetAllRoomData(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncGetAllRoomData(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncKickOffMember(RailID member_id, string user_data)
		{
			IntPtr intPtr = ((member_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (member_id != null)
			{
				RailConverter.Csharp2Cpp(member_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncKickOffMember(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult AsyncSetRoomTag(string room_tag, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncSetRoomTag(swigCPtr_, room_tag, user_data);
		}

		public virtual RailResult AsyncGetRoomTag(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncGetRoomTag(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncSetRoomMetadata(List<RailKeyValue> key_values, string user_data)
		{
			IntPtr intPtr = ((key_values == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			if (key_values != null)
			{
				RailConverter.Csharp2Cpp(key_values, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncSetRoomMetadata(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
		}

		public virtual RailResult AsyncGetRoomMetadata(List<string> keys, string user_data)
		{
			IntPtr intPtr = ((keys == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (keys != null)
			{
				RailConverter.Csharp2Cpp(keys, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncGetRoomMetadata(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult AsyncClearRoomMetadata(List<string> keys, string user_data)
		{
			IntPtr intPtr = ((keys == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (keys != null)
			{
				RailConverter.Csharp2Cpp(keys, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncClearRoomMetadata(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult AsyncSetMemberMetadata(RailID member_id, List<RailKeyValue> key_values, string user_data)
		{
			IntPtr intPtr = ((member_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (member_id != null)
			{
				RailConverter.Csharp2Cpp(member_id, intPtr);
			}
			IntPtr intPtr2 = ((key_values == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			if (key_values != null)
			{
				RailConverter.Csharp2Cpp(key_values, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncSetMemberMetadata(swigCPtr_, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr2);
			}
		}

		public virtual RailResult AsyncGetMemberMetadata(RailID member_id, List<string> keys, string user_data)
		{
			IntPtr intPtr = ((member_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (member_id != null)
			{
				RailConverter.Csharp2Cpp(member_id, intPtr);
			}
			IntPtr intPtr2 = ((keys == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (keys != null)
			{
				RailConverter.Csharp2Cpp(keys, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncGetMemberMetadata(swigCPtr_, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr2);
			}
		}

		public virtual RailResult SendDataToMember(RailID remote_peer, byte[] data_buf, uint data_len, uint message_type)
		{
			IntPtr intPtr = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_SendDataToMember__SWIG_0(swigCPtr_, intPtr, data_buf, data_len, message_type);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult SendDataToMember(RailID remote_peer, byte[] data_buf, uint data_len)
		{
			IntPtr intPtr = ((remote_peer == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (remote_peer != null)
			{
				RailConverter.Csharp2Cpp(remote_peer, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_SendDataToMember__SWIG_1(swigCPtr_, intPtr, data_buf, data_len);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult SetGameServerID(RailID game_server_rail_id)
		{
			IntPtr intPtr = ((game_server_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (game_server_rail_id != null)
			{
				RailConverter.Csharp2Cpp(game_server_rail_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_SetGameServerID(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailID GetGameServerID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailRoom_GetGameServerID(swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(ptr, railID);
			return railID;
		}

		public virtual RailResult SetRoomJoinable(bool is_joinable)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_SetRoomJoinable(swigCPtr_, is_joinable);
		}

		public virtual bool IsRoomJoinable()
		{
			return RAIL_API_PINVOKE.IRailRoom_IsRoomJoinable(swigCPtr_);
		}

		public virtual RailResult GetFriendsInRoom(List<RailID> friend_ids)
		{
			IntPtr intPtr = ((friend_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailRoom_GetFriendsInRoom(swigCPtr_, intPtr);
			}
			finally
			{
				if (friend_ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, friend_ids);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual bool IsUserInRoom(RailID user_rail_id)
		{
			IntPtr intPtr = ((user_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (user_rail_id != null)
			{
				RailConverter.Csharp2Cpp(user_rail_id, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailRoom_IsUserInRoom(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual RailResult EnableTeamVoice(bool enable)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_EnableTeamVoice(swigCPtr_, enable);
		}

		public virtual RailResult AsyncSetRoomType(EnumRoomType room_type, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncSetRoomType(swigCPtr_, (int)room_type, user_data);
		}

		public virtual RailResult AsyncSetRoomMaxMember(uint max_member, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailRoom_AsyncSetRoomMaxMember(swigCPtr_, max_member, user_data);
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
