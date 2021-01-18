namespace Epic.OnlineServices.Friends
{
	public class OnFriendsUpdateInfo
	{
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

		public FriendsStatus PreviousStatus
		{
			get;
			set;
		}

		public FriendsStatus CurrentStatus
		{
			get;
			set;
		}
	}
}
