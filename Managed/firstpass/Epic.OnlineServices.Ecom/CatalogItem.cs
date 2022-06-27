namespace Epic.OnlineServices.Ecom
{
	public class CatalogItem
	{
		public int ApiVersion => 1;

		public string CatalogNamespace { get; set; }

		public string Id { get; set; }

		public string EntitlementName { get; set; }

		public string TitleText { get; set; }

		public string DescriptionText { get; set; }

		public string LongDescriptionText { get; set; }

		public string TechnicalDetailsText { get; set; }

		public string DeveloperText { get; set; }

		public EcomItemType ItemType { get; set; }

		public long EntitlementEndTimestamp { get; set; }
	}
}
