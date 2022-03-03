using System.Collections.Generic;

namespace rail
{
	public class RailInGameCoinRequestCoinInfoResponse : EventBase
	{
		public List<RailCoinInfo> coin_infos = new List<RailCoinInfo>();
	}
}
