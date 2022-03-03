namespace rail
{
	public interface IRailUtils
	{
		uint GetTimeCountSinceGameLaunch();

		uint GetTimeCountSinceComputerLaunch();

		uint GetTimeFromServer();

		RailResult AsyncGetImageData(string image_path, uint scale_to_width, uint scale_to_height, string user_data);

		void GetErrorString(RailResult result, out string error_string);

		RailResult DirtyWordsFilter(string words, bool replace_sensitive, RailDirtyWordsCheckResult check_result);

		EnumRailPlatformType GetRailPlatformType();

		RailResult GetLaunchAppParameters(EnumRailLaunchAppType app_type, out string parameter);

		RailResult GetPlatformLanguageCode(out string language_code);

		RailResult SetWarningMessageCallback(RailWarningMessageCallbackFunction callback);

		RailResult GetCountryCodeOfCurrentLoggedInIP(out string country_code);
	}
}
