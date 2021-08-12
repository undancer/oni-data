namespace Epic.OnlineServices.Connect
{
	public class QueryProductUserIdMappingsOptions
	{
		public int ApiVersion => 2;

		public ProductUserId LocalUserId { get; set; }

		public ExternalAccountType AccountIdType_DEPRECATED { get; set; }

		public ProductUserId[] ProductUserIds { get; set; }
	}
}
