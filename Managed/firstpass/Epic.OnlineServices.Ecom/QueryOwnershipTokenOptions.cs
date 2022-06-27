namespace Epic.OnlineServices.Ecom
{
	public class QueryOwnershipTokenOptions
	{
		public int ApiVersion => 2;

		public EpicAccountId LocalUserId { get; set; }

		public string[] CatalogItemIds { get; set; }

		public string CatalogNamespace { get; set; }
	}
}
