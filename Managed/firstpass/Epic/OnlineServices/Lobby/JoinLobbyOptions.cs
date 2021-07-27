namespace Epic.OnlineServices.Lobby
{
	public class JoinLobbyOptions
	{
		public int ApiVersion => 2;

		public LobbyDetails LobbyDetailsHandle { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public bool PresenceEnabled { get; set; }
	}
}
