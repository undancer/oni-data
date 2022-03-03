using System.Collections.Generic;

namespace rail
{
	public class GameServerListFilter
	{
		public string tags_not_contained;

		public EnumRailOptionalValue filter_password;

		public string filter_game_server_name;

		public EnumRailOptionalValue filter_friends_created;

		public string tags_contained;

		public EnumRailOptionalValue filter_dedicated_server;

		public List<GameServerListFilterKey> filters = new List<GameServerListFilterKey>();

		public string filter_game_server_map;

		public string filter_game_server_host;

		public RailID owner_id = new RailID();
	}
}
