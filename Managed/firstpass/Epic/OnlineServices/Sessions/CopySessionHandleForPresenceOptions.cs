namespace Epic.OnlineServices.Sessions
{
	public class CopySessionHandleForPresenceOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }
	}
}
