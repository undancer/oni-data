namespace Epic.OnlineServices.Achievements
{
	public class OnUnlockAchievementsCompleteCallbackInfo
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

		public ProductUserId UserId
		{
			get;
			set;
		}

		public uint AchievementsCount
		{
			get;
			set;
		}
	}
}
