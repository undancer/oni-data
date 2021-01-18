namespace Epic.OnlineServices.P2P
{
	public class OnIncomingConnectionRequestInfo
	{
		public object ClientData
		{
			get;
			set;
		}

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
	}
}
