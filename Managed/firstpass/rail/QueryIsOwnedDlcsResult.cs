using System.Collections.Generic;

namespace rail
{
	public class QueryIsOwnedDlcsResult : EventBase
	{
		public List<RailDlcOwned> dlc_owned_list = new List<RailDlcOwned>();
	}
}
