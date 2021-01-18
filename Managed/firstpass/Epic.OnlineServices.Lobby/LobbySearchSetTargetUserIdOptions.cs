namespace Epic.OnlineServices.Lobby
{
	public class LobbySearchSetTargetUserIdOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId
		{
			get;
			set;
		}
	}
}
