namespace Epic.OnlineServices.Ecom
{
	public class CopyItemReleaseByIndexOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public string ItemId
		{
			get;
			set;
		}

		public uint ReleaseIndex
		{
			get;
			set;
		}
	}
}
