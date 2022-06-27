namespace Epic.OnlineServices.TitleStorage
{
	public class ReadFileOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId { get; set; }

		public string Filename { get; set; }

		public uint ReadChunkLengthBytes { get; set; }

		public OnReadFileDataCallback ReadFileDataCallback { get; set; }

		public OnFileTransferProgressCallback FileTransferProgressCallback { get; set; }
	}
}
