namespace Epic.OnlineServices.Presence
{
	public class PresenceChangedCallbackInfo
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

		public EpicAccountId PresenceUserId
		{
			get;
			set;
		}
	}
}
