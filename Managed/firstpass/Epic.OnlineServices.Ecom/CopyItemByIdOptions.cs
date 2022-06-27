namespace Epic.OnlineServices.Ecom
{
	public class CopyItemByIdOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public string ItemId { get; set; }
	}
}
