namespace Epic.OnlineServices.PlayerDataStorage
{
	public class GetFileMetadataCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }
	}
}
