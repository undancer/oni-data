namespace Epic.OnlineServices.Lobby
{
	public class LeaveLobbyOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }

		public string LobbyId { get; set; }
	}
}
