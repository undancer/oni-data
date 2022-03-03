namespace rail
{
	public class NotifyMetadataChange : EventBase
	{
		public RailID changer_id = new RailID();

		public ulong room_id;
	}
}
