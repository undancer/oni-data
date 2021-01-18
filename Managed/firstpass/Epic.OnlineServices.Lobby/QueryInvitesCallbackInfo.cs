namespace Epic.OnlineServices.Lobby
{
	public class QueryInvitesCallbackInfo
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

		public ProductUserId LocalUserId
		{
			get;
			set;
		}
	}
}
