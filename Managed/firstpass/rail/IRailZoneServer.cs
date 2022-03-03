using System.Collections.Generic;

namespace rail
{
	public interface IRailZoneServer : IRailComponent
	{
		RailZoneID GetZoneID();

		RailResult GetZoneNameLanguages(List<string> languages);

		RailResult GetZoneName(string language_filter, out string zone_name);

		RailResult GetZoneDescriptionLanguages(List<string> languages);

		RailResult GetZoneDescription(string language_filter, out string zone_description);

		RailResult GetGameServerAddresses(List<string> server_addresses);

		RailResult GetZoneMetadatas(List<RailKeyValue> key_values);

		RailResult GetChildrenZoneIDs(List<RailZoneID> zone_ids);

		bool IsZoneVisiable();

		bool IsZoneJoinable();

		uint GetZoneEnableStartTime();

		uint GetZoneEnableEndTime();
	}
}
