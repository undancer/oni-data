using System.Collections.Generic;

namespace rail
{
	public class GetMemberMetadataResult : EventBase
	{
		public List<RailKeyValue> key_value = new List<RailKeyValue>();

		public ulong room_id;

		public RailID member_id = new RailID();
	}
}
