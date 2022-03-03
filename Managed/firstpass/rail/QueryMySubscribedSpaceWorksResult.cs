using System.Collections.Generic;

namespace rail
{
	public class QueryMySubscribedSpaceWorksResult
	{
		public uint total_available_works;

		public EnumRailSpaceWorkType spacework_type;

		public List<RailSpaceWorkDescriptor> spacework_descriptors = new List<RailSpaceWorkDescriptor>();
	}
}
