namespace Epic.OnlineServices.Sessions
{
	public class SessionSearchSetTargetUserIdOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId
		{
			get;
			set;
		}
	}
}
