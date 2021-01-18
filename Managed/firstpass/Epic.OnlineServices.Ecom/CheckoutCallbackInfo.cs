namespace Epic.OnlineServices.Ecom
{
	public class CheckoutCallbackInfo
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

		public string TransactionId
		{
			get;
			set;
		}
	}
}
