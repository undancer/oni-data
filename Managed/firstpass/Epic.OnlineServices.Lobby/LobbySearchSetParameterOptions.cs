namespace Epic.OnlineServices.Lobby
{
	public class LobbySearchSetParameterOptions
	{
		public int ApiVersion => 1;

		public AttributeData Parameter { get; set; }

		public ComparisonOp ComparisonOp { get; set; }
	}
}
