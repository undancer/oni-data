namespace Epic.OnlineServices.UserInfo
{
	public class QueryUserInfoByExternalAccountCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public EpicAccountId LocalUserId { get; set; }

		public string ExternalAccountId { get; set; }

		public ExternalAccountType AccountType { get; set; }

		public EpicAccountId TargetUserId { get; set; }
	}
}
