using System.Collections.Generic;

namespace rail
{
	public class RailSpaceWorkSearchFilter
	{
		public string search_text;

		public bool match_all_required_tags;

		public List<RailKeyValue> required_metadata = new List<RailKeyValue>();

		public bool match_all_required_metadata;

		public List<string> required_tags = new List<string>();

		public List<RailKeyValue> excluded_metadata = new List<RailKeyValue>();

		public List<string> excluded_tags = new List<string>();
	}
}
