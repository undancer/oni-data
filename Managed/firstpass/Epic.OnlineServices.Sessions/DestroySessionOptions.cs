namespace Epic.OnlineServices.Sessions
{
	public class DestroySessionOptions
	{
		public int ApiVersion => 1;

		public string SessionName { get; set; }
	}
}
