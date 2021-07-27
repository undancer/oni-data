namespace Epic.OnlineServices.TitleStorage
{
	public class FileMetadata
	{
		public int ApiVersion => 1;

		public uint FileSizeBytes { get; set; }

		public string MD5Hash { get; set; }

		public string Filename { get; set; }
	}
}
