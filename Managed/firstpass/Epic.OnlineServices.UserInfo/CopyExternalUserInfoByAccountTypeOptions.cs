namespace Epic.OnlineServices.UserInfo
{
	public class CopyExternalUserInfoByAccountTypeOptions
	{
		public int ApiVersion => 1;

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

		public ExternalAccountType AccountType
		{
			get;
			set;
		}
	}
}
