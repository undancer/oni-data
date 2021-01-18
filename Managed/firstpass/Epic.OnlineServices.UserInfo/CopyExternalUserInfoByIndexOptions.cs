namespace Epic.OnlineServices.UserInfo
{
	public class CopyExternalUserInfoByIndexOptions
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

		public uint Index
		{
			get;
			set;
		}
	}
}
