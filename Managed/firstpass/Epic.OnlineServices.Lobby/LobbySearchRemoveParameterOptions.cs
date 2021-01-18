namespace Epic.OnlineServices.Lobby
{
	public class LobbySearchRemoveParameterOptions
	{
		public int ApiVersion => 1;

		public string Key
		{
			get;
			set;
		}

		public ComparisonOp ComparisonOp
		{
			get;
			set;
		}
	}
}
