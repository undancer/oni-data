namespace Epic.OnlineServices.Sessions
{
	public class UpdateSessionModificationOptions
	{
		public int ApiVersion => 1;

		public string SessionName
		{
			get;
			set;
		}
	}
}
