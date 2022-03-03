using System.Collections.Generic;

namespace rail
{
	public class UpdateAssetsPropertyFinished : EventBase
	{
		public List<RailAssetProperty> asset_property_list = new List<RailAssetProperty>();
	}
}
