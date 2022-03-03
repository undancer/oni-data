using System.Collections.Generic;

namespace rail
{
	public class RailQueryGameActivityResult : EventBase
	{
		public List<RailGameActivityInfo> game_activities = new List<RailGameActivityInfo>();
	}
}
