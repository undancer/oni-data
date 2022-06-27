namespace Epic.OnlineServices.Lobby
{
	public class LobbySearchSetMaxResultsOptions
	{
		public int ApiVersion => 1;

		public uint MaxResults { get; set; }
	}
}
