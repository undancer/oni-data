namespace Epic.OnlineServices.Sessions
{
	public class UpdateSessionCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public string SessionName { get; set; }

		public string SessionId { get; set; }
	}
}
