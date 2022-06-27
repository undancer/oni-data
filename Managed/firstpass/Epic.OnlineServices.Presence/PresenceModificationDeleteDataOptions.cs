namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationDeleteDataOptions
	{
		public int ApiVersion => 1;

		public PresenceModificationDataRecordId[] Records { get; set; }
	}
}
