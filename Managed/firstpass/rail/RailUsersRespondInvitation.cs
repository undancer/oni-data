namespace rail
{
	public class RailUsersRespondInvitation : EventBase
	{
		public RailInviteOptions original_invite_option = new RailInviteOptions();

		public EnumRailUsersInviteResponseType response;

		public RailID inviter_id = new RailID();
	}
}
