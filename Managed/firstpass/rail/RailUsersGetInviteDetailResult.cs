namespace rail
{
	public class RailUsersGetInviteDetailResult : EventBase
	{
		public string command_line;

		public EnumRailUsersInviteType invite_type;

		public RailID inviter_id = new RailID();
	}
}
