namespace Epic.OnlineServices.Lobby
{
	public class LobbyDetailsGetMemberAttributeCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId
		{
			get;
			set;
		}
	}
}
