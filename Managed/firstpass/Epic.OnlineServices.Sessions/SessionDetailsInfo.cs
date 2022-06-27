namespace Epic.OnlineServices.Sessions
{
	public class SessionDetailsInfo
	{
		public int ApiVersion => 1;

		public string SessionId { get; set; }

		public string HostAddress { get; set; }

		public uint NumOpenPublicConnections { get; set; }

		public SessionDetailsSettings Settings { get; set; }
	}
}
