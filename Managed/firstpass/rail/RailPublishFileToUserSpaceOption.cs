using System.Collections.Generic;

namespace rail
{
	public class RailPublishFileToUserSpaceOption
	{
		public RailKeyValue key_value = new RailKeyValue();

		public string description;

		public List<string> tags = new List<string>();

		public EnumRailSpaceWorkShareLevel level;

		public string version;

		public string preview_path_filename;

		public EnumRailSpaceWorkType type;

		public string space_work_name;
	}
}
