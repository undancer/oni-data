namespace rail
{
	public interface IRailGlobalAchievement : IRailComponent
	{
		RailResult AsyncRequestAchievement(string user_data);

		RailResult GetGlobalAchievedPercent(string name, out double percent);

		RailResult GetGlobalAchievedPercentDescending(int index, out string name, out double percent);
	}
}
