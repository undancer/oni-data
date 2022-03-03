namespace rail
{
	public interface IRailZoneServerHelper
	{
		RailZoneID GetPlayerSelectedZoneID();

		RailZoneID GetRootZoneID();

		IRailZoneServer OpenZoneServer(RailZoneID zone_id, out RailResult result);

		RailResult AsyncSwitchPlayerSelectedZone(RailZoneID zone_id);
	}
}
