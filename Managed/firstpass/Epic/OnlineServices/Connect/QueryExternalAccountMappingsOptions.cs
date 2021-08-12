namespace Epic.OnlineServices.Connect
{
	public class QueryExternalAccountMappingsOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }

		public ExternalAccountType AccountIdType { get; set; }

		public string[] ExternalAccountIds { get; set; }
	}
}
