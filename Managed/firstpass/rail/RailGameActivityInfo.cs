using System.Collections.Generic;

namespace rail
{
	public class RailGameActivityInfo
	{
		public ulong activity_id;

		public List<RailKeyValue> metadata_key_values = new List<RailKeyValue>();

		public uint end_time;

		public uint begin_time;

		public string activity_name;

		public string activity_description;
	}
}
