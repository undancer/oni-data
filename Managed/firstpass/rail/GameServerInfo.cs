using System.Collections.Generic;

namespace rail
{
	public class GameServerInfo
	{
		public List<RailKeyValue> server_kvs = new List<RailKeyValue>();

		public RailID owner_rail_id = new RailID();

		public string game_server_name;

		public string server_fullname;

		public bool is_dedicated;

		public string server_info;

		public string server_tags;

		public string spectator_host;

		public string server_description;

		public string server_host;

		public RailID game_server_rail_id = new RailID();

		public bool has_password;

		public string server_version;

		public List<string> server_mods = new List<string>();

		public uint bot_players;

		public string game_server_map;

		public uint max_players;

		public uint current_players;

		public bool is_friend_only;
	}
}
