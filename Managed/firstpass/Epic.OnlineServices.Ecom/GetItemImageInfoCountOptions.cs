namespace Epic.OnlineServices.Ecom
{
	public class GetItemImageInfoCountOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public string ItemId
		{
			get;
			set;
		}
	}
}
