using System.Collections.Generic;

namespace rail
{
	public class GetGameServerMetadataResult : EventBase
	{
		public RailID game_server_id = new RailID();

		public List<RailKeyValue> key_value = new List<RailKeyValue>();
	}
}
