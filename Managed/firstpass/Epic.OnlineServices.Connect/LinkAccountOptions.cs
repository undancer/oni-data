namespace Epic.OnlineServices.Connect
{
	public class LinkAccountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId LocalUserId
		{
			get;
			set;
		}

		public ContinuanceToken ContinuanceToken
		{
			get;
			set;
		}
	}
}
