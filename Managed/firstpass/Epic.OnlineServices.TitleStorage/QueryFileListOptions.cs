namespace Epic.OnlineServices.TitleStorage
{
	public class QueryFileListOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}

		public string[] ListOfTags
		{
			get;
			set;
		}
	}
}
