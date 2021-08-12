namespace Epic.OnlineServices.Stats
{
	public class IngestData
	{
		public int ApiVersion => 1;

		public string StatName { get; set; }

		public int IngestAmount { get; set; }
	}
}
