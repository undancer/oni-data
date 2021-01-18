namespace Epic.OnlineServices.Sessions
{
	public class RegisterPlayersOptions
	{
		public int ApiVersion => 1;

		public string SessionName
		{
			get;
			set;
		}

		public ProductUserId[] PlayersToRegister
		{
			get;
			set;
		}
	}
}
