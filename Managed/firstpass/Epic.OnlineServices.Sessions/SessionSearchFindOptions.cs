namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchFindOptions
	{
		public int ApiVersion => 2;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}
	}
}
