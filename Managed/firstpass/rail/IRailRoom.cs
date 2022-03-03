using System.Collections.Generic;

namespace rail
{
	public interface IRailRoom : IRailComponent
	{
		RailResult AsyncJoinRoom(string password, string user_data);

		ulong GetRoomID();

		RailResult GetRoomName(out string name);

		RailID GetOwnerID();

		bool HasPassword();

		EnumRoomType GetRoomType();

		uint GetMembers();

		RailID GetMemberByIndex(uint index);

		RailResult GetMemberNameByIndex(uint index, out string name);

		uint GetMaxMembers();

		void Leave();

		RailResult AsyncSetNewRoomOwner(RailID new_owner_id, string user_data);

		RailResult AsyncGetRoomMembers(string user_data);

		RailResult AsyncGetAllRoomData(string user_data);

		RailResult AsyncKickOffMember(RailID member_id, string user_data);

		RailResult AsyncSetRoomTag(string room_tag, string user_data);

		RailResult AsyncGetRoomTag(string user_data);

		RailResult AsyncSetRoomMetadata(List<RailKeyValue> key_values, string user_data);

		RailResult AsyncGetRoomMetadata(List<string> keys, string user_data);

		RailResult AsyncClearRoomMetadata(List<string> keys, string user_data);

		RailResult AsyncSetMemberMetadata(RailID member_id, List<RailKeyValue> key_values, string user_data);

		RailResult AsyncGetMemberMetadata(RailID member_id, List<string> keys, string user_data);

		RailResult SendDataToMember(RailID remote_peer, byte[] data_buf, uint data_len, uint message_type);

		RailResult SendDataToMember(RailID remote_peer, byte[] data_buf, uint data_len);

		RailResult SetGameServerID(RailID game_server_rail_id);

		RailID GetGameServerID();

		RailResult SetRoomJoinable(bool is_joinable);

		bool IsRoomJoinable();

		RailResult GetFriendsInRoom(List<RailID> friend_ids);

		bool IsUserInRoom(RailID user_rail_id);

		RailResult EnableTeamVoice(bool enable);

		RailResult AsyncSetRoomType(EnumRoomType room_type, string user_data);

		RailResult AsyncSetRoomMaxMember(uint max_member, string user_data);
	}
}
