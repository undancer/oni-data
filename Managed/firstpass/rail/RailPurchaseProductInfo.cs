namespace rail
{
	public class RailPurchaseProductInfo
	{
		public string category;

		public float original_price;

		public string description;

		public RailDiscountInfo discount = new RailDiscountInfo();

		public bool is_purchasable;

		public string name;

		public string currency_type;

		public string product_thumbnail;

		public RailPurchaseProductExtraInfo extra_info = new RailPurchaseProductExtraInfo();

		public uint product_id;
	}
}
