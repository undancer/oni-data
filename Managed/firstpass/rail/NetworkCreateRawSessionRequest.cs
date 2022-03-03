namespace rail
{
	public class NetworkCreateRawSessionRequest : EventBase
	{
		public RailID local_peer = new RailID();

		public RailGamePeer remote_game_peer = new RailGamePeer();
	}
}
