namespace Epic.OnlineServices.Lobby
{
	public class LobbyMemberStatusReceivedCallbackInfo
	{
		public object ClientData { get; set; }

		public string LobbyId { get; set; }

		public ProductUserId TargetUserId { get; set; }

		public LobbyMemberStatus CurrentStatus { get; set; }
	}
}
