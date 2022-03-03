using System.Collections.Generic;

namespace rail
{
	public class MergeAssetsFinished : EventBase
	{
		public List<RailAssetItem> source_assets = new List<RailAssetItem>();

		public ulong new_asset_id;
	}
}
