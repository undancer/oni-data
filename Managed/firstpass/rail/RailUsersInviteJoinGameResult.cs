namespace rail
{
	public class RailUsersInviteJoinGameResult : EventBase
	{
		public EnumRailUsersInviteResponseType response_value;

		public RailID invitee_id = new RailID();

		public EnumRailUsersInviteType invite_type;
	}
}
