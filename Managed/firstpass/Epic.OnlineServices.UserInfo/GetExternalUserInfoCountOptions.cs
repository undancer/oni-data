namespace Epic.OnlineServices.UserInfo
{
	public class GetExternalUserInfoCountOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public EpicAccountId TargetUserId { get; set; }
	}
}
