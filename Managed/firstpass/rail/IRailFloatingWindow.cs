namespace rail
{
	public interface IRailFloatingWindow
	{
		RailResult AsyncShowRailFloatingWindow(EnumRailWindowType window_type, string user_data);

		RailResult AsyncCloseRailFloatingWindow(EnumRailWindowType window_type, string user_data);

		RailResult SetNotifyWindowPosition(EnumRailNotifyWindowType window_type, RailWindowLayout layout);

		RailResult AsyncShowStoreWindow(ulong id, RailStoreOptions options, string user_data);

		bool IsFloatingWindowAvailable();

		RailResult AsyncShowDefaultGameStoreWindow(string user_data);

		RailResult SetNotifyWindowEnable(EnumRailNotifyWindowType window_type, bool enable);
	}
}
