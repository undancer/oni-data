namespace Epic.OnlineServices.Connect
{
	public class GetProductUserExternalAccountCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId { get; set; }
	}
}
