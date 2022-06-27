namespace Epic.OnlineServices.Achievements
{
	public class Definition
	{
		public int ApiVersion => 1;

		public string AchievementId { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public string LockedDisplayName { get; set; }

		public string LockedDescription { get; set; }

		public string HiddenDescription { get; set; }

		public string CompletionDescription { get; set; }

		public string UnlockedIconId { get; set; }

		public string LockedIconId { get; set; }

		public bool IsHidden { get; set; }

		public StatThresholds[] StatThresholds { get; set; }
	}
}
