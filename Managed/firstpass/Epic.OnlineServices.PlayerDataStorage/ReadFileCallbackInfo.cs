namespace Epic.OnlineServices.PlayerDataStorage
{
	public class ReadFileCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public string Filename { get; set; }
	}
}
