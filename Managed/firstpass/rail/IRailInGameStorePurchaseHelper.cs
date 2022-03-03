namespace rail
{
	public interface IRailInGameStorePurchaseHelper
	{
		RailResult AsyncShowPaymentWindow(string order_id, string user_data);
	}
}
