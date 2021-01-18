namespace Epic.OnlineServices.UserInfo
{
	public class QueryUserInfoByDisplayNameCallbackInfo
	{
		public Result ResultCode
		{
			get;
			set;
		}

		public object ClientData
		{
			get;
			set;
		}

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

		public string DisplayName
		{
			get;
			set;
		}
	}
}
