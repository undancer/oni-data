namespace Epic.OnlineServices.Sessions
{
	public class SendInviteOptions
	{
		public int ApiVersion => 1;

		public string SessionName
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
