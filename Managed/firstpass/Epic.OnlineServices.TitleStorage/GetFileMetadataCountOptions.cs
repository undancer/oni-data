namespace Epic.OnlineServices.TitleStorage
{
	public class GetFileMetadataCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}
	}
}
