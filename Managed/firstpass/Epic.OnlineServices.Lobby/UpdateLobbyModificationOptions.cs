namespace Epic.OnlineServices.Lobby
{
	public class UpdateLobbyModificationOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}

		public string LobbyId
		{
			get;
			set;
		}
	}
}
