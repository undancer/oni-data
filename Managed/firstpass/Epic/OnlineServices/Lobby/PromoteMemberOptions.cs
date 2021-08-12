namespace Epic.OnlineServices.Lobby
{
	public class PromoteMemberOptions
	{
		public int ApiVersion => 1;

		public string LobbyId { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public ProductUserId TargetUserId { get; set; }
	}
}
