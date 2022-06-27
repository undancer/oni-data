namespace Epic.OnlineServices.Presence
{
	public class QueryPresenceCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }

		public EpicAccountId TargetUserId { get; set; }
	}
}
