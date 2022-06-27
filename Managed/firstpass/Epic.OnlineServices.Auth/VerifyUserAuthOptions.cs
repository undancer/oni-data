namespace Epic.OnlineServices.Auth
{
	public class VerifyUserAuthOptions
	{
		public int ApiVersion => 1;

		public Token AuthToken { get; set; }
	}
}
