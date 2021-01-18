namespace Epic.OnlineServices.Lobby
{
	public class RejectInviteCallbackInfo
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

		public string InviteId
		{
			get;
			set;
		}
	}
}
