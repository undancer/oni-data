namespace Epic.OnlineServices.Sessions
{
	public class JoinSessionOptions
	{
		public int ApiVersion => 2;

		public string SessionName { get; set; }

		public SessionDetails SessionHandle { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public bool PresenceEnabled { get; set; }
	}
}
