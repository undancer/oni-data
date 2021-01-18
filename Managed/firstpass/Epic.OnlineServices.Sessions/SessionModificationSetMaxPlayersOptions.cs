namespace Epic.OnlineServices.Sessions
{
	public class SessionModificationSetMaxPlayersOptions
	{
		public int ApiVersion => 1;

		public uint MaxPlayers
		{
			get;
			set;
		}
	}
}
