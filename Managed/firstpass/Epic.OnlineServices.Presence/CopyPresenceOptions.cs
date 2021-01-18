namespace Epic.OnlineServices.Presence
{
	public class CopyPresenceOptions
	{
		public int ApiVersion => 2;

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
