using System.Collections.Generic;

namespace rail
{
	public interface IRailGameServerHelper
	{
		RailResult AsyncGetGameServerPlayerList(RailID gameserver_rail_id, string user_data);

		RailResult AsyncGetGameServerList(uint start_index, uint end_index, List<GameServerListFilter> alternative_filters, List<GameServerListSorter> sorter, string user_data);

		IRailGameServer AsyncCreateGameServer(CreateGameServerOptions options, string game_server_name, string user_data);

		IRailGameServer AsyncCreateGameServer(CreateGameServerOptions options, string game_server_name);

		IRailGameServer AsyncCreateGameServer(CreateGameServerOptions options);

		IRailGameServer AsyncCreateGameServer();

		RailResult AsyncGetFavoriteGameServers(string user_data);

		RailResult AsyncGetFavoriteGameServers();

		RailResult AsyncAddFavoriteGameServer(RailID game_server_id, string user_data);

		RailResult AsyncAddFavoriteGameServer(RailID game_server_id);

		RailResult AsyncRemoveFavoriteGameServer(RailID game_server_id, string user_Data);

		RailResult AsyncRemoveFavoriteGameServer(RailID game_server_id);
	}
}
