namespace Epic.OnlineServices.Ecom
{
	public class QueryEntitlementsCallbackInfo
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

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}
	}
}
