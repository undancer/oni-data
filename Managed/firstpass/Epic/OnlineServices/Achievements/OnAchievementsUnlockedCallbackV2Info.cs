using System;

namespace Epic.OnlineServices.Achievements
{
	public class OnAchievementsUnlockedCallbackV2Info
	{
		public object ClientData { get; set; }

		public ProductUserId UserId { get; set; }

		public string AchievementId { get; set; }

		public DateTimeOffset? UnlockTime { get; set; }
	}
}
