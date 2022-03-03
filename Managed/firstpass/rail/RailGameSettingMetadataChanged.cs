using System.Collections.Generic;

namespace rail
{
	public class RailGameSettingMetadataChanged : EventBase
	{
		public List<RailKeyValue> key_values = new List<RailKeyValue>();

		public RailGameSettingMetadataChangedSource source;
	}
}
