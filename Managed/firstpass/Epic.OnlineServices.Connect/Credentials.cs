namespace Epic.OnlineServices.Connect
{
	public class Credentials
	{
		public int ApiVersion => 1;

		public string Token
		{
			get;
			set;
		}

		public ExternalCredentialType Type
		{
			get;
			set;
		}
	}
}
