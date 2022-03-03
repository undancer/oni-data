namespace rail
{
	public interface IRailApps
	{
		bool IsGameInstalled(RailGameID game_id);

		RailResult AsyncQuerySubscribeWishPlayState(RailGameID game_id, string user_data);
	}
}
