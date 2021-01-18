namespace Epic.OnlineServices.Ecom
{
	public class CopyTransactionByIdOptions
	{
		public int ApiVersion => 1;

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
