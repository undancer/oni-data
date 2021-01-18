namespace Epic.OnlineServices.Sessions
{
	public class JoinSessionAcceptedCallbackInfo
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

		public ulong UiEventId
		{
			get;
			set;
		}
	}
}
