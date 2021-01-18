namespace Epic.OnlineServices.P2P
{
	public class GetNextReceivedPacketSizeOptions
	{
		public int ApiVersion => 2;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}

		public byte? RequestedChannel
		{
			get;
			set;
		}
	}
}
