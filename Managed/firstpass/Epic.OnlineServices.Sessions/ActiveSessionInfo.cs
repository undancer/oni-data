namespace Epic.OnlineServices.Sessions
{
	public class ActiveSessionInfo
	{
		public int ApiVersion => 1;

		public string SessionName { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public OnlineSessionState State { get; set; }

		public SessionDetailsInfo SessionDetails { get; set; }
	}
}
