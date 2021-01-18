namespace Epic.OnlineServices.Achievements
{
	public class CopyAchievementDefinitionV2ByAchievementIdOptions
	{
		public int ApiVersion => 2;

		public string AchievementId
		{
			get;
			set;
		}
	}
}
