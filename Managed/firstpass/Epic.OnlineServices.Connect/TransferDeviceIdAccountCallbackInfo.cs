namespace Epic.OnlineServices.Connect
{
	public class TransferDeviceIdAccountCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }
	}
}
