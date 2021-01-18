namespace Epic.OnlineServices.Ecom
{
	public class GetEntitlementsByNameCountOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public string EntitlementName
		{
			get;
			set;
		}
	}
}
