namespace Epic.OnlineServices.Auth
{
	public class LinkAccountCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }

		public PinGrantInfo PinGrantInfo { get; set; }
	}
}
