namespace Epic.OnlineServices.Lobby
{
	public class LobbyInviteReceivedCallbackInfo
	{
		public object ClientData
		{
			get;
			set;
		}

		public string InviteId
		{
			get;
			set;
		}

		public ProductUserId LocalUserId
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
