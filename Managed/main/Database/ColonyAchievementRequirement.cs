namespace Database
{
	public abstract class ColonyAchievementRequirement
	{
		public abstract bool Success();

		public virtual bool Fail()
		{
			return false;
		}

		public virtual string GetProgress(bool complete)
		{
			return "";
		}
	}
}
