namespace Epic.OnlineServices.Metrics
{
	public class EndPlayerSessionOptions
	{
		public int ApiVersion => 1;

		public EndPlayerSessionOptionsAccountId AccountId { get; set; }
	}
}
