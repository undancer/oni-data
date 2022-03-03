namespace rail
{
	public interface IRailFactory
	{
		IRailPlayer RailPlayer();

		IRailUsersHelper RailUsersHelper();

		IRailFriends RailFriends();

		IRailFloatingWindow RailFloatingWindow();

		IRailBrowserHelper RailBrowserHelper();

		IRailInGamePurchase RailInGamePurchase();

		IRailInGameCoin RailInGameCoin();

		IRailRoomHelper RailRoomHelper();

		IRailGameServerHelper RailGameServerHelper();

		IRailStorageHelper RailStorageHelper();

		IRailUserSpaceHelper RailUserSpaceHelper();

		IRailStatisticHelper RailStatisticHelper();

		IRailLeaderboardHelper RailLeaderboardHelper();

		IRailAchievementHelper RailAchievementHelper();

		IRailNetwork RailNetworkHelper();

		IRailApps RailApps();

		IRailGame RailGame();

		IRailUtils RailUtils();

		IRailAssetsHelper RailAssetsHelper();

		IRailDlcHelper RailDlcHelper();

		IRailScreenshotHelper RailScreenshotHelper();

		IRailVoiceHelper RailVoiceHelper();

		IRailSystemHelper RailSystemHelper();

		IRailTextInputHelper RailTextInputHelper();

		IRailIMEHelper RailIMETextInputHelper();

		IRailHttpSessionHelper RailHttpSessionHelper();

		IRailSmallObjectServiceHelper RailSmallObjectServiceHelper();

		IRailZoneServerHelper RailZoneServerHelper();

		IRailGroupChatHelper RailGroupChatHelper();

		IRailInGameStorePurchaseHelper RailInGameStorePurchaseHelper();

		IRailInGameActivityHelper RailInGameActivityHelper();

		IRailAntiAddictionHelper RailAntiAddictionHelper();

		IRailThirdPartyAccountLoginHelper RailThirdPartyAccountLoginHelper();
	}
}
