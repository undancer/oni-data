namespace Epic.OnlineServices.P2P
{
	public class SendPacketOptions
	{
		public int ApiVersion => 2;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}

		public ProductUserId RemoteUserId
		{
			get;
			set;
		}

		public SocketId SocketId
		{
			get;
			set;
		}

		public byte Channel
		{
			get;
			set;
		}

		public byte[] Data
		{
			get;
			set;
		}

		public bool AllowDelayedDelivery
		{
			get;
			set;
		}

		public PacketReliability Reliability
		{
			get;
			set;
		}
	}
}
