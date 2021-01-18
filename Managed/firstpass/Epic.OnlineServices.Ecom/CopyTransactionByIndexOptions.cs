namespace Epic.OnlineServices.Ecom
{
	public class CopyTransactionByIndexOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public uint TransactionIndex
		{
			get;
			set;
		}
	}
}
