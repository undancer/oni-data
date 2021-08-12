namespace Epic.OnlineServices.Ecom
{
	public class CopyOfferImageInfoByIndexOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public string OfferId { get; set; }

		public uint ImageInfoIndex { get; set; }
	}
}
