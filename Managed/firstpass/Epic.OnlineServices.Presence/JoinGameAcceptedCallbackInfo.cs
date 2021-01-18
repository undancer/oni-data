namespace Epic.OnlineServices.Presence
{
	public class JoinGameAcceptedCallbackInfo
	{
		public object ClientData
		{
			get;
			set;
		}

		public string JoinInfo
		{
			get;
			set;
		}

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public EpicAccountId TargetUserId
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
