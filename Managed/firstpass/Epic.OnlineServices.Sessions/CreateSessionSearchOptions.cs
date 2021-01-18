namespace Epic.OnlineServices.Sessions
{
	public class CreateSessionSearchOptions
	{
		public int ApiVersion => 1;

		public uint MaxSearchResults
		{
			get;
			set;
		}
	}
}
