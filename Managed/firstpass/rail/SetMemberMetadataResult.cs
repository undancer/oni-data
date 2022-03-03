namespace rail
{
	public class SetMemberMetadataResult : EventBase
	{
		public ulong room_id;

		public RailID member_id = new RailID();
	}
}
