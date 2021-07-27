namespace Epic.OnlineServices.TitleStorage
{
	public class QueryFileListCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public ProductUserId LocalUserId { get; set; }

		public uint FileCount { get; set; }
	}
}
