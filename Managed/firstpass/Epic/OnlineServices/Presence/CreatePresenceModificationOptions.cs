namespace Epic.OnlineServices.Presence
{
	public class CreatePresenceModificationOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }
	}
}
