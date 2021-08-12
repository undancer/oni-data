namespace Epic.OnlineServices.Lobby
{
	public class LobbyModificationSetMaxMembersOptions
	{
		public int ApiVersion => 1;

		public uint MaxMembers { get; set; }
	}
}
