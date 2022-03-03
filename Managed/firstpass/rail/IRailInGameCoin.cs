namespace rail
{
	public interface IRailInGameCoin
	{
		RailResult AsyncRequestCoinInfo(string user_data);

		RailResult AsyncPurchaseCoins(RailCoins purchase_info, string user_data);
	}
}
