using System.Collections.Generic;

namespace rail
{
	public class GetUserRoomListResult : EventBase
	{
		public List<RoomInfo> room_info = new List<RoomInfo>();
	}
}
