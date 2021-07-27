namespace Epic.OnlineServices.Sessions
{
	public class SessionDetailsSettings
	{
		public int ApiVersion => 2;

		public string BucketId { get; set; }

		public uint NumPublicConnections { get; set; }

		public bool AllowJoinInProgress { get; set; }

		public OnlineSessionPermissionLevel PermissionLevel { get; set; }

		public bool InvitesAllowed { get; set; }
	}
}
