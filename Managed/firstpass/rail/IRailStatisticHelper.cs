namespace rail
{
	public interface IRailStatisticHelper
	{
		IRailPlayerStats CreatePlayerStats(RailID player);

		IRailGlobalStats GetGlobalStats();

		RailResult AsyncGetNumberOfPlayer(string user_data);
	}
}
