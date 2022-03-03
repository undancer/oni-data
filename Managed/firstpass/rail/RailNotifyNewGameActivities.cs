using System.Collections.Generic;

namespace rail
{
	public class RailNotifyNewGameActivities : EventBase
	{
		public List<RailGameActivityInfo> game_activities = new List<RailGameActivityInfo>();
	}
}
