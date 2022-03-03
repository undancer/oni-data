namespace rail
{
	public class NotifyRoomGameServerChange : EventBase
	{
		public RailID game_server_rail_id = new RailID();

		public ulong room_id;

		public ulong game_server_channel_id;
	}
}
