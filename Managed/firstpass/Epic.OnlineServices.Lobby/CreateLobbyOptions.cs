namespace Epic.OnlineServices.Lobby
{
	public class CreateLobbyOptions
	{
		public int ApiVersion => 2;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}

		public uint MaxLobbyMembers
		{
			get;
			set;
		}

		public LobbyPermissionLevel PermissionLevel
		{
			get;
			set;
		}

		public bool PresenceEnabled
		{
			get;
			set;
		}
	}
}
