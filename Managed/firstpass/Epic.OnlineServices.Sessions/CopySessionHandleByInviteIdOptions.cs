namespace Epic.OnlineServices.Sessions
{
	public class CopySessionHandleByInviteIdOptions
	{
		public int ApiVersion => 1;

		public string InviteId { get; set; }
	}
}
