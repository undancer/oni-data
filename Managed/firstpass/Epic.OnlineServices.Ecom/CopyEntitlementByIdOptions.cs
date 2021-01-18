namespace Epic.OnlineServices.Ecom
{
	public class CopyEntitlementByIdOptions
	{
		public int ApiVersion => 2;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public string EntitlementId
		{
			get;
			set;
		}
	}
}
