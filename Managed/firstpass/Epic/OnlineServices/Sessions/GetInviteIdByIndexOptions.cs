namespace Epic.OnlineServices.Sessions
{
	public class GetInviteIdByIndexOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }

		public uint Index { get; set; }
	}
}
