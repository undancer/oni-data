namespace Epic.OnlineServices.Lobby
{
	public class CreateLobbySearchOptions
	{
		public int ApiVersion => 1;

		public uint MaxResults
		{
			get;
			set;
		}
	}
}
