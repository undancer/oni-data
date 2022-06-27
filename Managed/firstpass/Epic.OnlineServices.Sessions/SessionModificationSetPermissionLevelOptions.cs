namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetPermissionLevelOptions
	{
		public int ApiVersion => 1;

		public OnlineSessionPermissionLevel PermissionLevel { get; set; }
	}
}
