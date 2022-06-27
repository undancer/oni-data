namespace Epic.OnlineServices.Presence
{
	public class SetPresenceOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public PresenceModification PresenceModificationHandle { get; set; }
	}
}
