namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetInvitesAllowedOptions
	{
		public int ApiVersion => 1;

		public bool InvitesAllowed
		{
			get;
			set;
		}
	}
}
