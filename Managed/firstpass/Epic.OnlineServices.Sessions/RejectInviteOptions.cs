namespace Epic.OnlineServices.Sessions
{
	public class RejectInviteOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
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
