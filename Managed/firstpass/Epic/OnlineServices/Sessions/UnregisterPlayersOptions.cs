namespace Epic.OnlineServices.Sessions
{
	public class UnregisterPlayersOptions
	{
		public int ApiVersion => 1;

		public string SessionName { get; set; }

		public ProductUserId[] PlayersToUnregister { get; set; }
	}
}
