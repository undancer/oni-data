using System.Collections.Generic;

namespace rail
{
	public interface IRailRoomHelper
	{
		IRailRoom CreateRoom(RoomOptions options, string room_name, out RailResult result);

		IRailRoom AsyncCreateRoom(RoomOptions options, string room_name, string user_data);

		IRailRoom OpenRoom(ulong room_id, out RailResult result);

		IRailRoom AsyncOpenRoom(ulong room_id, string user_data);

		RailResult AsyncGetRoomList(uint start_index, uint end_index, List<RoomInfoListSorter> sorter, List<RoomInfoListFilter> filter, string user_data);

		RailResult AsyncGetRoomListByTags(uint start_index, uint end_index, List<RoomInfoListSorter> sorter, List<string> room_tags, string user_data);

		RailResult AsyncGetUserRoomList(string user_data);
	}
}
