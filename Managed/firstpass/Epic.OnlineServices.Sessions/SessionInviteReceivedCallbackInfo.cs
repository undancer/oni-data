namespace Epic.OnlineServices.Sessions
{
	public class SessionInviteReceivedCallbackInfo
	{
		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public ProductUserId TargetUserId { get; set; }

		public string InviteId { get; set; }
	}
}
