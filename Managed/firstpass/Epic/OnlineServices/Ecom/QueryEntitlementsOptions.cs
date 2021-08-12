namespace Epic.OnlineServices.Ecom
{
	public class QueryEntitlementsOptions
	{
		public int ApiVersion => 2;

		public EpicAccountId LocalUserId { get; set; }

		public string[] EntitlementNames { get; set; }

		public bool IncludeRedeemed { get; set; }
	}
}
