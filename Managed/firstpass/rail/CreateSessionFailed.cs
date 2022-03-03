namespace rail
{
	public class CreateSessionFailed : EventBase
	{
		public RailID local_peer = new RailID();

		public RailID remote_peer = new RailID();
	}
}
