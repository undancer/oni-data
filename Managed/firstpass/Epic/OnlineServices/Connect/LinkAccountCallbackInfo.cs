namespace Epic.OnlineServices.Connect
{
	public class LinkAccountCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }
	}
}
