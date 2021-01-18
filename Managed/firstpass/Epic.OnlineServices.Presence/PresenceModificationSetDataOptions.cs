namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationSetDataOptions
	{
		public int ApiVersion => 1;

		public DataRecord[] Records
		{
			get;
			set;
		}
	}
}
