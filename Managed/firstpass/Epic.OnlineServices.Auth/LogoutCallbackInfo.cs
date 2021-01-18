namespace Epic.OnlineServices.Auth
{
	public class LogoutCallbackInfo
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

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}
	}
}
