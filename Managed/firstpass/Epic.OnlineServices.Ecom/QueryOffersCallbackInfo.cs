namespace Epic.OnlineServices.Ecom
{
	public class QueryOffersCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }
	}
}
