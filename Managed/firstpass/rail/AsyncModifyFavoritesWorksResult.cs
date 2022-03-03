using System.Collections.Generic;

namespace rail
{
	public class AsyncModifyFavoritesWorksResult : EventBase
	{
		public List<SpaceWorkID> success_ids = new List<SpaceWorkID>();

		public List<SpaceWorkID> failure_ids = new List<SpaceWorkID>();

		public EnumRailModifyFavoritesSpaceWorkType modify_flag;
	}
}
