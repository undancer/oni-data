namespace Epic.OnlineServices.Ecom
{
	public class CopyEntitlementByIndexOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public uint EntitlementIndex
		{
			get;
			set;
		}
	}
}
