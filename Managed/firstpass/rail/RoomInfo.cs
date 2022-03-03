using System.Collections.Generic;

namespace rail
{
	public class RoomInfo
	{
		public bool has_password;

		public uint max_members;

		public string room_name;

		public RailID game_server_rail_id = new RailID();

		public uint create_time;

		public uint current_members;

		public EnumRoomType type;

		public bool is_joinable;

		public ulong room_id;

		public List<RailKeyValue> room_kvs = new List<RailKeyValue>();

		public string room_tag;

		public RailID owner_id = new RailID();
	}
}
