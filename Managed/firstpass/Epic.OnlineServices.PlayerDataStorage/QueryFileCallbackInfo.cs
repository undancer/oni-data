namespace Epic.OnlineServices.PlayerDataStorage
{
	public class QueryFileCallbackInfo
	{
		public Result ResultCode
		{
			get;
			set;
		}

		public object ClientData
		{
			get;
			set;
		}

		public ProductUserId LocalUserId
		{
			get;
			set;
		}
	}
}
