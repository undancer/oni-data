using System.Collections.Generic;

namespace rail
{
	public class RailInGamePurchaseRequestAllPurchasableProductsResponse : EventBase
	{
		public List<RailPurchaseProductInfo> purchasable_products = new List<RailPurchaseProductInfo>();
	}
}
