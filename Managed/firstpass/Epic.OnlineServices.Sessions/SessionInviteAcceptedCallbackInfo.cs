namespace Epic.OnlineServices.Sessions
{
	public class SessionInviteAcceptedCallbackInfo
	{
		public object ClientData { get; set; }

		public string SessionId { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public ProductUserId TargetUserId { get; set; }

		public string InviteId { get; set; }
	}
}
