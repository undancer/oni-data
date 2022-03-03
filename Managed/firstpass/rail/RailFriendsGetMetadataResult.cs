using System.Collections.Generic;

namespace rail
{
	public class RailFriendsGetMetadataResult : EventBase
	{
		public RailID friend_id = new RailID();

		public List<RailKeyValueResult> friend_kvs = new List<RailKeyValueResult>();
	}
}
