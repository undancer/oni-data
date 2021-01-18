namespace Epic.OnlineServices.Lobby
{
	public class UpdateLobbyOptions
	{
		public int ApiVersion => 1;

		public LobbyModification LobbyModificationHandle
		{
			get;
			set;
		}
	}
}
