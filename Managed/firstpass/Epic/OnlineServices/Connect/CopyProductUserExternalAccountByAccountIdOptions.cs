namespace Epic.OnlineServices.Connect
{
	public class CopyProductUserExternalAccountByAccountIdOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId { get; set; }

		public string AccountId { get; set; }
	}
}
