namespace Epic.OnlineServices.Stats
{
	public class CopyStatByNameOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId { get; set; }

		public string Name { get; set; }
	}
}
