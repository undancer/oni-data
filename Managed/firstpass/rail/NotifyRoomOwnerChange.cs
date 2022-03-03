namespace rail
{
	public class NotifyRoomOwnerChange : EventBase
	{
		public RailID old_owner_id = new RailID();

		public EnumRoomOwnerChangeReason reason;

		public ulong room_id;

		public RailID new_owner_id = new RailID();
	}
}
