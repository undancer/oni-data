namespace Epic.OnlineServices.Connect
{
	public class LoginOptions
	{
		public int ApiVersion => 2;

		public Credentials Credentials { get; set; }

		public UserLoginInfo UserLoginInfo { get; set; }
	}
}
