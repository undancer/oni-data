namespace Epic.OnlineServices.Connect
{
	public class LoginStatusChangedCallbackInfo
	{
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

		public LoginStatus PreviousStatus
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
