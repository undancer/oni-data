namespace Epic.OnlineServices.Connect
{
	public class UnlinkAccountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}
	}
}
