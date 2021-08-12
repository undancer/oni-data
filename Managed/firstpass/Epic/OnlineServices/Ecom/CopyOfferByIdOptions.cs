namespace Epic.OnlineServices.Ecom
{
	public class CopyOfferByIdOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public string OfferId { get; set; }
	}
}
