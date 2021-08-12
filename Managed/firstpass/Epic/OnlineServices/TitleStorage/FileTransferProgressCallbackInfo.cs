namespace Epic.OnlineServices.TitleStorage
{
	public class FileTransferProgressCallbackInfo
	{
		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public string Filename { get; set; }

		public uint BytesTransferred { get; set; }

		public uint TotalFileSizeBytes { get; set; }
	}
}
