namespace Epic.OnlineServices.P2P
{
	public class OnQueryNATTypeCompleteInfo
	{
		public Result ResultCode
		{
			get;
			set;
		}

		public object ClientData
		{
			get;
			set;
		}

		public NATType NATType
		{
			get;
			set;
		}
	}
}
