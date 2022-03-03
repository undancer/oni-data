namespace rail
{
	public class NetworkCreateRawSessionFailed : EventBase
	{
		public RailID local_peer = new RailID();

		public RailGamePeer remote_game_peer = new RailGamePeer();
	}
}
