namespace Epic.OnlineServices.PlayerDataStorage
{
	public class DuplicateFileCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }
	}
}
