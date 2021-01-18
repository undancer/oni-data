namespace Epic.OnlineServices.Achievements
{
	public class DefinitionV2
	{
		public int ApiVersion => 2;

		public string AchievementId
		{
			get;
			set;
		}

		public string UnlockedDisplayName
		{
			get;
			set;
		}

		public string UnlockedDescription
		{
			get;
			set;
		}

		public string LockedDisplayName
		{
			get;
			set;
		}

		public string LockedDescription
		{
			get;
			set;
		}

		public string FlavorText
		{
			get;
			set;
		}

		public string UnlockedIconURL
		{
			get;
			set;
		}

		public string LockedIconURL
		{
			get;
			set;
		}

		public bool IsHidden
		{
			get;
			set;
		}

		public StatThresholds[] StatThresholds
		{
			get;
			set;
		}
	}
}
