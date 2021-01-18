namespace Epic.OnlineServices.Achievements
{
	public class CopyAchievementDefinitionByIndexOptions
	{
		public int ApiVersion => 1;

		public uint AchievementIndex
		{
			get;
			set;
		}
	}
}
