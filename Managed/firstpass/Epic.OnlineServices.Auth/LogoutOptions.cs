namespace Epic.OnlineServices.Auth
{
	public class LogoutOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}
	}
}
