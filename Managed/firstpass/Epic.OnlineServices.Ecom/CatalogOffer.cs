namespace Epic.OnlineServices.Ecom
{
	public class CatalogOffer
	{
		public int ApiVersion => 2;

		public int ServerIndex { get; set; }

		public string CatalogNamespace { get; set; }

		public string Id { get; set; }

		public string TitleText { get; set; }

		public string DescriptionText { get; set; }

		public string LongDescriptionText { get; set; }

		public string TechnicalDetailsText_DEPRECATED { get; set; }

		public string CurrencyCode { get; set; }

		public Result PriceResult { get; set; }

		public uint OriginalPrice { get; set; }

		public uint CurrentPrice { get; set; }

		public byte DiscountPercentage { get; set; }

		public long ExpirationTimestamp { get; set; }

		public uint PurchasedCount { get; set; }

		public int PurchaseLimit { get; set; }

		public bool AvailableForPurchase { get; set; }
	}
}
