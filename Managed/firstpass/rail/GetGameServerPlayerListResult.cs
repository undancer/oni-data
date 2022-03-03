using System.Collections.Generic;

namespace rail
{
	public class GetGameServerPlayerListResult : EventBase
	{
		public RailID game_server_id = new RailID();

		public List<GameServerPlayerInfo> server_player_info = new List<GameServerPlayerInfo>();
	}
}
