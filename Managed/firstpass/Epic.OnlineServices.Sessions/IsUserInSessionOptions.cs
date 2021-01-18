namespace Epic.OnlineServices.Sessions
{
	public class IsUserInSessionOptions
	{
		public int ApiVersion => 1;

		public string SessionName
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
