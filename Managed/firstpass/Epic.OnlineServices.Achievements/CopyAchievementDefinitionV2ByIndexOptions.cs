namespace Epic.OnlineServices.Achievements
{
	public class CopyAchievementDefinitionV2ByIndexOptions
	{
		public int ApiVersion => 2;

		public uint AchievementIndex
		{
			get;
			set;
		}
	}
}
