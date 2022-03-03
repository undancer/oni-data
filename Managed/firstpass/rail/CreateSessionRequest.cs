namespace rail
{
	public class CreateSessionRequest : EventBase
	{
		public RailID local_peer = new RailID();

		public RailID remote_peer = new RailID();
	}
}
