using System.Collections.Generic;

namespace rail
{
	public class RailFriendPlayedGameInfo
	{
		public bool in_room;

		public List<ulong> room_id_list = new List<ulong>();

		public RailID friend_id = new RailID();

		public List<ulong> game_server_id_list = new List<ulong>();

		public RailGameID game_id = new RailGameID();

		public bool in_game_server;

		public RailFriendPlayedGamePlayState friend_played_game_play_state;
	}
}
