namespace Epic.OnlineServices.Sessions
{
	public class GetInviteCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }
	}
}
