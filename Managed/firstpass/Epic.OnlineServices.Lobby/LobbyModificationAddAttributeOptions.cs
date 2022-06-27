namespace Epic.OnlineServices.Lobby
{
	public class LobbyModificationAddAttributeOptions
	{
		public int ApiVersion => 1;

		public AttributeData Attribute { get; set; }

		public LobbyAttributeVisibility Visibility { get; set; }
	}
}
