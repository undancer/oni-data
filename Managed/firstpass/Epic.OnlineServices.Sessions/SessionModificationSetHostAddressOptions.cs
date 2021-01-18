namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetHostAddressOptions
	{
		public int ApiVersion => 1;

		public string HostAddress
		{
			get;
			set;
		}
	}
}
