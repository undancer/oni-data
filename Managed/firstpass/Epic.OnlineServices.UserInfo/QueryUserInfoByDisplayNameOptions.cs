namespace Epic.OnlineServices.UserInfo
{
	public class QueryUserInfoByDisplayNameOptions
	{
		public int ApiVersion => 1;

		public EpicAccountId LocalUserId
		{
			get;
			set;
		}

		public string DisplayName
		{
			get;
			set;
		}
	}
}
