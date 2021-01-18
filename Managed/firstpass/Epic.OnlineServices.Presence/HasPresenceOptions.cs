namespace Epic.OnlineServices.Presence
{
	public class HasPresenceOptions
	{
		public int ApiVersion => 1;

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
	}
}
