namespace Epic.OnlineServices.Lobby
{
	public class CopyLobbyDetailsHandleByInviteIdOptions
	{
		public int ApiVersion => 1;

		public string InviteId { get; set; }
	}
}
