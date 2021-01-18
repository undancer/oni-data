namespace Epic.OnlineServices.Ecom
{
	public class GetTransactionCountOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}
	}
}
