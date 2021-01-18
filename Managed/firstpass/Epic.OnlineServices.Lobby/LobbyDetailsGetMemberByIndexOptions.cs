namespace Epic.OnlineServices.Lobby
{
	public class LobbyDetailsGetMemberByIndexOptions
	{
		public int ApiVersion => 1;

		public uint MemberIndex
		{
			get;
			set;
		}
	}
}
