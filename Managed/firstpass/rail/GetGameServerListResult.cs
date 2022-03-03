using System.Collections.Generic;

namespace rail
{
	public class GetGameServerListResult : EventBase
	{
		public List<GameServerInfo> server_info = new List<GameServerInfo>();

		public uint total_num;

		public uint start_index;

		public uint end_index;
	}
}
