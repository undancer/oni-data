namespace Epic.OnlineServices.Ecom
{
	public class ItemOwnership
	{
		public int ApiVersion => 1;

		public string Id
		{
			get;
			set;
		}

		public OwnershipStatus OwnershipStatus
		{
			get;
			set;
		}
	}
}
