using System.Collections.Generic;

namespace rail
{
	public class ExchangeAssetsFinished : EventBase
	{
		public List<RailAssetItem> old_assets = new List<RailAssetItem>();

		public List<RailGeneratedAssetItem> new_asset_item_list = new List<RailGeneratedAssetItem>();
	}
}
