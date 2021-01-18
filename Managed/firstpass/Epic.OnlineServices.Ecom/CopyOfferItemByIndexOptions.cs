namespace Epic.OnlineServices.Ecom
{
	public class CopyOfferItemByIndexOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public string OfferId
		{
			get;
			set;
		}

		public uint ItemIndex
		{
			get;
			set;
		}
	}
}
