using System.Collections.Generic;

namespace rail
{
	public class RailInGamePurchasePurchaseProductsToAssetsResponse : EventBase
	{
		public string order_id;

		public List<RailAssetInfo> delivered_assets = new List<RailAssetInfo>();
	}
}
