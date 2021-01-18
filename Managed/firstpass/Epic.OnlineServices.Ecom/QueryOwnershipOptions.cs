namespace Epic.OnlineServices.Ecom
{
	public class QueryOwnershipOptions
	{
		public int ApiVersion => 2;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public string[] CatalogItemIds
		{
			get;
			set;
		}

		public string CatalogNamespace
		{
			get;
			set;
		}
	}
}
