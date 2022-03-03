using System.Collections.Generic;

namespace rail
{
	public interface IRailGame
	{
		RailGameID GetGameID();

		RailResult ReportGameContentDamaged(EnumRailGameContentDamageFlag flag);

		RailResult GetGameInstallPath(out string app_path);

		RailResult AsyncQuerySubscribeWishPlayState(string user_data);

		RailResult GetPlayerSelectedLanguageCode(out string language_code);

		RailResult GetGameSupportedLanguageCodes(List<string> language_codes);

		RailResult SetGameState(EnumRailGamePlayingState game_state);

		RailResult GetGameState(out EnumRailGamePlayingState game_state);

		RailResult RegisterGameDefineGamePlayingState(List<RailGameDefineGamePlayingState> game_playing_states);

		RailResult SetGameDefineGamePlayingState(uint game_playing_state);

		RailResult GetGameDefineGamePlayingState(out uint game_playing_state);

		RailResult GetBranchBuildNumber(out string build_number);

		RailResult GetCurrentBranchInfo(RailBranchInfo branch_info);

		RailResult StartGameTimeCounting(string counting_key);

		RailResult EndGameTimeCounting(string counting_key);

		RailID GetGamePurchasePlayerRailID();

		uint GetGameEarliestPurchaseTime();

		uint GetTimeCountSinceGameActivated();

		uint GetTimeCountSinceLastMouseMoved();
	}
}
