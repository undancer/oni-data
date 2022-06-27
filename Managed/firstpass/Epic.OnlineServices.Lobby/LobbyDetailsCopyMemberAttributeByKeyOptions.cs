namespace Epic.OnlineServices.Lobby
{
	public class LobbyDetailsCopyMemberAttributeByKeyOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId { get; set; }

		public string AttrKey { get; set; }
	}
}
