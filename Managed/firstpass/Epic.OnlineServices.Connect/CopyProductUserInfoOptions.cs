namespace Epic.OnlineServices.Connect
{
	public class CopyProductUserInfoOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId
		{
			get;
			set;
		}
	}
}
