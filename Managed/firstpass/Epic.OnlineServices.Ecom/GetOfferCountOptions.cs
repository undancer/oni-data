namespace Epic.OnlineServices.Ecom
{
	public class GetOfferCountOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}
	}
}
