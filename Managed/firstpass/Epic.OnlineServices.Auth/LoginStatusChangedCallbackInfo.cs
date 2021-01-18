namespace Epic.OnlineServices.Auth
{
	public class LoginStatusChangedCallbackInfo
	{
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

		public LoginStatus PrevStatus
		{
			get;
			set;
		}

		public LoginStatus CurrentStatus
		{
			get;
			set;
		}
	}
}
