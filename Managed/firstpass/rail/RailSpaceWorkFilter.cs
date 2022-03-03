using System.Collections.Generic;

namespace rail
{
	public class RailSpaceWorkFilter
	{
		public List<EnumRailWorkFileClass> classes = new List<EnumRailWorkFileClass>();

		public List<EnumRailSpaceWorkType> type = new List<EnumRailSpaceWorkType>();

		public List<RailID> collector_list = new List<RailID>();

		public List<RailID> subscriber_list = new List<RailID>();

		public List<RailID> creator_list = new List<RailID>();
	}
}
