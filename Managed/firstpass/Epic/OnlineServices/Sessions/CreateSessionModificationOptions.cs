namespace Epic.OnlineServices.Sessions
{
	public class CreateSessionModificationOptions
	{
		public int ApiVersion => 3;

		public string SessionName { get; set; }

		public string BucketId { get; set; }

		public uint MaxPlayers { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public bool PresenceEnabled { get; set; }

		public string SessionId { get; set; }
	}
}
