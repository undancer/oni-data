namespace rail
{
	public class KickOffMemberResult : EventBase
	{
		public ulong room_id;

		public RailID kicked_id = new RailID();
	}
}
