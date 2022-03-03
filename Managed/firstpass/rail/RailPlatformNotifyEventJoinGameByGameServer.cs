namespace rail
{
	public class RailPlatformNotifyEventJoinGameByGameServer : EventBase
	{
		public string commandline_info;

		public RailID gameserver_railid = new RailID();
	}
}
