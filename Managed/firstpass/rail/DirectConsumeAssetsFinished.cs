using System.Collections.Generic;

namespace rail
{
	public class DirectConsumeAssetsFinished : EventBase
	{
		public List<RailAssetItem> assets = new List<RailAssetItem>();
	}
}
