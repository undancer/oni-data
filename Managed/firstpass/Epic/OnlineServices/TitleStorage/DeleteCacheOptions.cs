namespace Epic.OnlineServices.TitleStorage
{
	public class DeleteCacheOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }
	}
}
