using System.Collections.Generic;

namespace rail
{
	public class RoomMemberInfo
	{
		public string member_name;

		public uint member_index;

		public ulong room_id;

		public List<RailKeyValue> member_kvs = new List<RailKeyValue>();

		public RailID member_id = new RailID();
	}
}
