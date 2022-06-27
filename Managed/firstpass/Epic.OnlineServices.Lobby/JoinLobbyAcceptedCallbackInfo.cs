namespace Epic.OnlineServices.Lobby
{
	public class JoinLobbyAcceptedCallbackInfo
	{
		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public ulong UiEventId { get; set; }
	}
}
