namespace Epic.OnlineServices.Friends
{
	public class RejectInviteOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public EpicAccountId TargetUserId { get; set; }
	}
}
