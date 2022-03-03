namespace rail
{
	public interface IRailPlayerStats : IRailComponent
	{
		RailID GetRailID();

		RailResult AsyncRequestStats(string user_data);

		RailResult GetStatValue(string name, out int data);

		RailResult GetStatValue(string name, out double data);

		RailResult SetStatValue(string name, int data);

		RailResult SetStatValue(string name, double data);

		RailResult UpdateAverageStatValue(string name, double data);

		RailResult AsyncStoreStats(string user_data);

		RailResult ResetAllStats();
	}
}
