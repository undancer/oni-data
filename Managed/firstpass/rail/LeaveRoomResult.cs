namespace rail
{
	public class LeaveRoomResult : EventBase
	{
		public EnumLeaveRoomReason reason;

		public ulong room_id;
	}
}
