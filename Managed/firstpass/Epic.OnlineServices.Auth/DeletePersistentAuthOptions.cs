namespace Epic.OnlineServices.Auth
{
	public class DeletePersistentAuthOptions
	{
		public int ApiVersion => 2;

		public string RefreshToken
		{
			get;
			set;
		}
	}
}
