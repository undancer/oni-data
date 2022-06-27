namespace Epic.OnlineServices.Friends
{
	public class GetStatusOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public EpicAccountId TargetUserId { get; set; }
	}
}
