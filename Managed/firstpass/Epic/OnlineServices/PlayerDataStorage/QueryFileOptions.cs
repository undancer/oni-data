namespace Epic.OnlineServices.PlayerDataStorage
{
	public class QueryFileOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }

		public string Filename { get; set; }
	}
}
