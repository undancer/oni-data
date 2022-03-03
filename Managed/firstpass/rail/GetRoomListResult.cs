using System.Collections.Generic;

namespace rail
{
	public class GetRoomListResult : EventBase
	{
		public List<RoomInfo> room_infos = new List<RoomInfo>();

		public uint total_room_num;

		public uint begin_index;

		public uint end_index;
	}
}
