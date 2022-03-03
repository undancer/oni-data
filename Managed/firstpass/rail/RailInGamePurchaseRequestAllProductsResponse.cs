using System.Collections.Generic;

namespace rail
{
	public class RailInGamePurchaseRequestAllProductsResponse : EventBase
	{
		public List<RailPurchaseProductInfo> all_products = new List<RailPurchaseProductInfo>();
	}
}
