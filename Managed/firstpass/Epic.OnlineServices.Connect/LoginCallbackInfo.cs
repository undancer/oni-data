namespace Epic.OnlineServices.Connect
{
	public class LoginCallbackInfo
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

		public ContinuanceToken ContinuanceToken
		{
			get;
			set;
		}
	}
}
