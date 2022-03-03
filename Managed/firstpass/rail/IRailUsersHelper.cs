using System.Collections.Generic;

namespace rail
{
	public interface IRailUsersHelper
	{
		RailResult AsyncGetUsersInfo(List<RailID> rail_ids, string user_data);

		RailResult AsyncInviteUsers(string command_line, List<RailID> users, RailInviteOptions options, string user_data);

		RailResult AsyncGetInviteDetail(RailID inviter, EnumRailUsersInviteType invite_type, string user_data);

		RailResult AsyncCancelInvite(EnumRailUsersInviteType invite_type, string user_data);

		RailResult AsyncCancelAllInvites(string user_data);

		RailResult AsyncGetUserLimits(RailID user_id, string user_data);

		RailResult AsyncShowChatWindowWithFriend(RailID rail_id, string user_data);

		RailResult AsyncShowUserHomepageWindow(RailID rail_id, string user_data);
	}
}
