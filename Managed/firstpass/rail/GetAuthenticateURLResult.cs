namespace rail
{
	public class GetAuthenticateURLResult : EventBase
	{
		public uint ticket_expire_time;

		public string authenticate_url;

		public string source_url;
	}
}
