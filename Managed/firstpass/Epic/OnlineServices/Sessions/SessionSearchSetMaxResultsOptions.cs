namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchSetMaxResultsOptions
	{
		public int ApiVersion => 1;

		public uint MaxSearchResults { get; set; }
	}
}
