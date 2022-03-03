using System.Collections.Generic;

namespace rail
{
	public class ExchangeAssetsToFinished : EventBase
	{
		public ulong exchange_to_asset_id;

		public RailProductItem to_product_info = new RailProductItem();

		public List<RailAssetItem> old_assets = new List<RailAssetItem>();
	}
}
