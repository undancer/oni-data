using System.Collections.Generic;

namespace rail
{
	public class GetRoomMetadataResult : EventBase
	{
		public List<RailKeyValue> key_value = new List<RailKeyValue>();

		public ulong room_id;
	}
}
