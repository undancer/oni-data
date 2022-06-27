namespace Epic.OnlineServices.Ecom
{
	public class QueryOwnershipCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }

		public ItemOwnership[] ItemOwnership { get; set; }
	}
}
