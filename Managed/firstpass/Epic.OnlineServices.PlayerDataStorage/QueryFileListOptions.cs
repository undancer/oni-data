namespace Epic.OnlineServices.PlayerDataStorage
{
	public class QueryFileListOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}
	}
}
