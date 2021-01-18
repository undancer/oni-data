namespace Epic.OnlineServices.Lobby
{
	public class CopyLobbyDetailsHandleByUiEventIdOptions
	{
		public int ApiVersion => 1;

		public ulong UiEventId
		{
			get;
			set;
		}
	}
}
