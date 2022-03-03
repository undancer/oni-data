namespace rail
{
	public class RailPlatformNotifyEventJoinGameByUser : EventBase
	{
		public RailID rail_id_to_join = new RailID();

		public string commandline_info;
	}
}
