namespace Epic.OnlineServices.Lobby
{
	public class RejectInviteOptions
	{
		public int ApiVersion => 1;

		public string InviteId { get; set; }

		public ProductUserId LocalUserId { get; set; }
	}
}
