namespace Epic.OnlineServices.Lobby
{
	public class SendInviteCallbackInfo
	{
		public Result ResultCode
		{
			get;
			set;
		}

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
	}
}
