namespace Epic.OnlineServices.Lobby
{
	public class LobbyModificationSetPermissionLevelOptions
	{
		public int ApiVersion => 1;

		public LobbyPermissionLevel PermissionLevel
		{
			get;
			set;
		}
	}
}
