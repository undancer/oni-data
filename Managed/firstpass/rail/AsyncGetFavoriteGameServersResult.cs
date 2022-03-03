using System.Collections.Generic;

namespace rail
{
	public class AsyncGetFavoriteGameServersResult : EventBase
	{
		public List<RailID> server_id_array = new List<RailID>();
	}
}
