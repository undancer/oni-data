namespace Epic.OnlineServices.Ecom
{
	public class QueryOffersOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public string OverrideCatalogNamespace { get; set; }
	}
}
