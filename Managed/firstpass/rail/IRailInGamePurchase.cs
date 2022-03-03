using System.Collections.Generic;

namespace rail
{
	public interface IRailInGamePurchase
	{
		RailResult AsyncRequestAllPurchasableProducts(string user_data);

		RailResult AsyncRequestAllProducts(string user_data);

		RailResult GetProductInfo(uint product_id, RailPurchaseProductInfo product);

		RailResult AsyncPurchaseProducts(List<RailProductItem> cart_items, string user_data);

		RailResult AsyncFinishOrder(string order_id, string user_data);

		RailResult AsyncPurchaseProductsToAssets(List<RailProductItem> cart_items, string user_data);
	}
}
