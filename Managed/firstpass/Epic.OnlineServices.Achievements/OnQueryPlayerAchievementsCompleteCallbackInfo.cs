namespace Epic.OnlineServices.Achievements
{
	public class OnQueryPlayerAchievementsCompleteCallbackInfo
	{
		public Result ResultCode { get; set; }

		public object ClientData { get; set; }

		public ProductUserId UserId { get; set; }
	}
}
