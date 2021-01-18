namespace Epic.OnlineServices.Ecom
{
	public class CopyOfferByIndexOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public uint OfferIndex
		{
			get;
			set;
		}
	}
}
