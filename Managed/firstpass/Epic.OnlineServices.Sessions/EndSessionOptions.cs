namespace Epic.OnlineServices.Sessions
{
	public class EndSessionOptions
	{
		public int ApiVersion => 1;

		public string SessionName
		{
			get;
			set;
		}
	}
}
