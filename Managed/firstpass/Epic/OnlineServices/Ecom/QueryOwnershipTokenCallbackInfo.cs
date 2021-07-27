namespace Epic.OnlineServices.Ecom
{
	public class QueryOwnershipTokenCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }

		public string OwnershipToken { get; set; }
	}
}
