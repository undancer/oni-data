namespace Epic.OnlineServices.UserInfo
{
	public class CopyUserInfoOptions
	{
		public int ApiVersion => 2;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public EpicAccountId TargetUserId
		{
			get;
			set;
		}
	}
}
