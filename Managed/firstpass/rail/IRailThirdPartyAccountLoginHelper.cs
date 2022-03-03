namespace rail
{
	public interface IRailThirdPartyAccountLoginHelper
	{
		RailResult AsyncAutoLogin(string user_data);

		RailResult AsyncLogin(RailThirdPartyAccountLoginOptions options, string user_data);

		RailResult GetAccountInfo(RailThirdPartyAccountInfo account_info);
	}
}
