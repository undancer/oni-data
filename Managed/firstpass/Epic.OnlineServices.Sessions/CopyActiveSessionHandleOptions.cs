namespace Epic.OnlineServices.Sessions
{
	public class CopyActiveSessionHandleOptions
	{
		public int ApiVersion => 1;

		public string SessionName
		{
			get;
			set;
		}
	}
}
