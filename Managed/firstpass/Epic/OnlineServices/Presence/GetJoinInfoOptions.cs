namespace Epic.OnlineServices.Presence
{
	public class GetJoinInfoOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public EpicAccountId TargetUserId { get; set; }
	}
}
