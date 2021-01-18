namespace Epic.OnlineServices.Connect
{
	public class TransferDeviceIdAccountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId PrimaryLocalUserId
		{
			get;
			set;
		}

		public ProductUserId LocalDeviceUserId
		{
			get;
			set;
		}

		public ProductUserId ProductUserIdToPreserve
		{
			get;
			set;
		}
	}
}
