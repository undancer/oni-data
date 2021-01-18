namespace Epic.OnlineServices.Stats
{
	public class GetStatCountOptions
	{
		public int ApiVersion => 1;

		public ProductUserId TargetUserId
		{
			get;
			set;
		}
	}
}
