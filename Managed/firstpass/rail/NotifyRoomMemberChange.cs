namespace rail
{
	public class NotifyRoomMemberChange : EventBase
	{
		public RailID changer_id = new RailID();

		public RailID id_for_making_change = new RailID();

		public EnumRoomMemberActionStatus state_change;

		public ulong room_id;
	}
}
