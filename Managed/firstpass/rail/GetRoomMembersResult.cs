using System.Collections.Generic;

namespace rail
{
	public class GetRoomMembersResult : EventBase
	{
		public List<RoomMemberInfo> member_infos = new List<RoomMemberInfo>();

		public ulong room_id;

		public uint member_num;
	}
}
