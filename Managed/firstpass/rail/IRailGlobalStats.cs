namespace rail
{
	public interface IRailGlobalStats : IRailComponent
	{
		RailResult AsyncRequestGlobalStats(string user_data);

		RailResult GetGlobalStatValue(string name, out long data);

		RailResult GetGlobalStatValue(string name, out double data);

		RailResult GetGlobalStatValueHistory(string name, long[] global_stats_data, uint data_size, out int num_global_stats);

		RailResult GetGlobalStatValueHistory(string name, double[] global_stats_data, uint data_size, out int num_global_stats);
	}
}
