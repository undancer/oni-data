namespace Epic.OnlineServices.UserInfo
{
	public class QueryUserInfoByExternalAccountOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId { get; set; }

		public string ExternalAccountId { get; set; }

		public ExternalAccountType AccountType { get; set; }
	}
}
