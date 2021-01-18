namespace Epic.OnlineServices.Ecom
{
	public class RedeemEntitlementsOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public string[] EntitlementIds
		{
			get;
			set;
		}
	}
}
