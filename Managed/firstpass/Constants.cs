using UnityEngine;

public static class Constants
{
	public const float GlobalTimeScale = 1f;

	public const float MINUTES = 60f;

	public const float SECONDS_PER_CYCLE = 600f;

	public const int ScheduleBlocksPerCycle = 24;

	public const int NIGHT_BLOCKS = 3;

	public const int DAY_BLOCKS = 21;

	public const float SecondsPerScheduleBlock = 25f;

	public const float NightimeDurationInSeconds = 75f;

	public const float DaytimeDurationInSeconds = 525f;

	public const float NightimeDurationInPercentage = 0.125f;

	public const float DaytimeDurationInPercentage = 0.875f;

	public const float StartTimeInSeconds = 50f;

	public const float CircuitOverloadTime = 6f;

	public const int AutoSaveDayInterval = 1;

	public const float ICE_DIG_TIME = 4f;

	public const string BULLETSTRING = "• ";

	public const string TABSTRING = "    ";

	public const string TABBULLETSTRING = "    • ";

	public static readonly Color POSITIVE_COLOR = new Color(82f / 255f, 64f / 85f, 121f / 255f);

	public static readonly string POSITIVE_COLOR_STR = "#" + POSITIVE_COLOR.ToHexString();

	public static readonly Color NEGATIVE_COLOR = new Color(244f / 255f, 74f / 255f, 71f / 255f);

	public static readonly string NEGATIVE_COLOR_STR = "#" + NEGATIVE_COLOR.ToHexString();

	public static readonly Color NEUTRAL_COLOR = Color.grey;

	public static readonly string NEUTRAL_COLOR_STR = "#" + NEUTRAL_COLOR.ToHexString();

	public static readonly Color WARNING_COLOR = new Color(1f, 14f / 15f, 22f / 51f);

	public static readonly string WARNING_COLOR_STR = "#" + WARNING_COLOR.ToHexString();

	public static readonly string WHITE_COLOR_STR = "#" + Color.white.ToHexString();

	public const float W2KW = 0.001f;

	public const float KW2W = 1000f;

	public const float J2DTU = 1f;

	public const float W2DTU_S = 1f;

	public const float KW2DTU_S = 1000f;

	public const float G2KG = 0.001f;

	public const float KG2G = 1000f;

	public const float CELSIUS2KELVIN = 273.15f;

	public const float CAL2KCAL = 0.001f;

	public const float KCAL2CAL = 1000f;

	public const float HEATOFVAPORIZATION_WATER = 580f;

	public const float DefaultEntityThickness = 0.01f;

	public const float DefaultSurfaceArea = 10f;

	public const float DefaultGroundTransferScale = 0.0625f;

	public const float SPACE_DISTANCE_TO_KILOMETERS = 10000f;

	public const int LOGIC_SOUND_INTERVAL_COUNTER = 2;

	public const float LOGIC_SOUND_VOLUME_COOLDOWN = 3f;

	public const float DEFAULT_SOUND_EVENT_VOLUME = 1f;

	public const float AUDIO_HIGHLIGHT_POSITION_INTENSITY = 0f;
}
