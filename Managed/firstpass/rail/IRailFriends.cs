using System.Collections.Generic;

namespace rail
{
	public interface IRailFriends
	{
		RailResult AsyncGetPersonalInfo(List<RailID> rail_ids, string user_data);

		RailResult AsyncGetFriendMetadata(RailID rail_id, List<string> keys, string user_data);

		RailResult AsyncSetMyMetadata(List<RailKeyValue> key_values, string user_data);

		RailResult AsyncClearAllMyMetadata(string user_data);

		RailResult AsyncSetInviteCommandLine(string command_line, string user_data);

		RailResult AsyncGetInviteCommandLine(RailID rail_id, string user_data);

		RailResult AsyncReportPlayedWithUserList(List<RailUserPlayedWith> player_list, string user_data);

		RailResult GetFriendsList(List<RailFriendInfo> friends_list);

		RailResult AsyncQueryFriendPlayedGamesInfo(RailID rail_id, string user_data);

		RailResult AsyncQueryPlayedWithFriendsList(string user_data);

		RailResult AsyncQueryPlayedWithFriendsTime(List<RailID> rail_ids, string user_data);

		RailResult AsyncQueryPlayedWithFriendsGames(List<RailID> rail_ids, string user_data);

		RailResult AsyncAddFriend(RailFriendsAddFriendRequest request, string user_data);

		RailResult AsyncUpdateFriendsData(string user_data);
	}
}
