namespace Epic.OnlineServices.UI
{
	public class HideFriendsCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }
	}
}
