namespace Epic.OnlineServices.PlayerDataStorage
{
	public class DuplicateFileOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }

		public string SourceFilename { get; set; }

		public string DestinationFilename { get; set; }
	}
}
