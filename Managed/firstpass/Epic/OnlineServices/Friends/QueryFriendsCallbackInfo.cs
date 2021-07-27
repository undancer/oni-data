namespace Epic.OnlineServices.Friends
{
	public class QueryFriendsCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }
	}
}
