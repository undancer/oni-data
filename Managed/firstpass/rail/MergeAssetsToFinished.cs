using System.Collections.Generic;

namespace rail
{
	public class MergeAssetsToFinished : EventBase
	{
		public ulong merge_to_asset_id;

		public List<RailAssetItem> source_assets = new List<RailAssetItem>();
	}
}
