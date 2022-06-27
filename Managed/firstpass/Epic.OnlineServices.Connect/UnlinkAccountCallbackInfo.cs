namespace Epic.OnlineServices.Connect
{
	public class UnlinkAccountCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }
	}
}
