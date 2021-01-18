namespace Epic.OnlineServices.P2P
{
	public class CloseConnectionsOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
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
