namespace rail
{
	public class RoomDataReceived : EventBase
	{
		public uint data_len;

		public RailID remote_peer = new RailID();

		public uint message_type;

		public string data_buf;
	}
}
