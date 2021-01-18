namespace Epic.OnlineServices.Achievements
{
	public class CopyAchievementDefinitionByAchievementIdOptions
	{
		public int ApiVersion => 1;

		public string AchievementId
		{
			get;
			set;
		}
	}
}
