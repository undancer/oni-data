using System.Collections.Generic;

namespace rail
{
	public class RequestAllAssetsFinished : EventBase
	{
		public List<RailAssetInfo> assetinfo_list = new List<RailAssetInfo>();
	}
}
