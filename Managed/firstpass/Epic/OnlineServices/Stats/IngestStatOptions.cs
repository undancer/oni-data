namespace Epic.OnlineServices.Stats
{
	public class IngestStatOptions
	{
		public int ApiVersion => 2;

		public ProductUserId LocalUserId { get; set; }

		public IngestData[] Stats { get; set; }

		public ProductUserId TargetUserId { get; set; }
	}
}
