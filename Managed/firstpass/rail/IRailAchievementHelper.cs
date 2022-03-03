namespace rail
{
	public interface IRailAchievementHelper
	{
		IRailPlayerAchievement CreatePlayerAchievement(RailID player);

		IRailGlobalAchievement GetGlobalAchievement();
	}
}
