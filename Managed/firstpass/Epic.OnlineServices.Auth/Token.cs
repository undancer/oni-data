namespace Epic.OnlineServices.Auth
{
	public class Token
	{
		public int ApiVersion => 2;

		public string App
		{
			get;
			set;
		}

		public string ClientId
		{
			get;
			set;
		}

		public EpicAccountId AccountId
		{
			get;
			set;
		}

		public string AccessToken
		{
			get;
			set;
		}

		public double ExpiresIn
		{
			get;
			set;
		}

		public string ExpiresAt
		{
			get;
			set;
		}

		public AuthTokenType AuthType
		{
			get;
			set;
		}

		public string RefreshToken
		{
			get;
			set;
		}

		public double RefreshExpiresIn
		{
			get;
			set;
		}

		public string RefreshExpiresAt
		{
			get;
			set;
		}
	}
}
