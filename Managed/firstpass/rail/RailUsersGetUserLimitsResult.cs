using System.Collections.Generic;

namespace rail
{
	public class RailUsersGetUserLimitsResult : EventBase
	{
		public RailID user_id = new RailID();

		public List<EnumRailUsersLimits> user_limits = new List<EnumRailUsersLimits>();
	}
}
