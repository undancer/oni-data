using System.Collections.Generic;

namespace rail
{
	public class RoomInfoListFilter
	{
		public EnumRailOptionalValue filter_friends_in_room;

		public string room_tag;

		public uint available_slot_at_least;

		public EnumRailOptionalValue filter_password;

		public string room_name_contained;

		public List<RoomInfoListFilterKey> key_filters = new List<RoomInfoListFilterKey>();

		public EnumRailOptionalValue filter_friends_owned;
	}
}
