namespace rail
{
	public interface IRailInGameActivityHelper
	{
		RailResult AsyncQueryGameActivity(string user_data);

		RailResult AsyncOpenDefaultGameActivityWindow(string user_data);

		RailResult AsyncOpenGameActivityWindow(ulong activity_id, string user_data);
	}
}
