namespace Epic.OnlineServices.Lobby
{
	public class LobbyMemberUpdateReceivedCallbackInfo
	{
		public object ClientData
		{
			get;
			set;
		}

		public string LobbyId
		{
			get;
			set;
		}

		public ProductUserId TargetUserId
		{
			get;
			set;
		}
	}
}
