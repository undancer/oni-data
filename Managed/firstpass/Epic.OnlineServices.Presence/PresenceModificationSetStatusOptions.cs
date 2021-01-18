namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationSetStatusOptions
	{
		public int ApiVersion => 1;

		public Status Status
		{
			get;
			set;
		}
	}
}
