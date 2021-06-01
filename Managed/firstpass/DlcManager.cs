using System.Collections.Generic;

public class DlcManager
{
	public const string PACK1_ID = "PACK1";

	public const string VANILLA_ID = "";

	public const string EXPANSION1_ID = "EXPANSION1_ID";

	public static List<string> RELEASE_ORDER = new List<string>
	{
		"",
		"EXPANSION1_ID"
	};

	public static bool IsAheadOfInstalledDlc(string dlcId)
	{
		return IsAheadOf(dlcId, GetInstalledDlcId());
	}

	public static bool IsVanillaId(string dlcId)
	{
		if (dlcId != null)
		{
			return dlcId == "";
		}
		return true;
	}

	public static string GetContentBundleName(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return "expansion1_bundle";
		}
		Debug.LogError("No bundle exists for " + dlcId);
		return null;
	}

	public static string GetContentDirectoryName(string dlcId)
	{
		if (dlcId == "EXPANSION1_ID")
		{
			return "expansion1";
		}
		Debug.LogError("No content directory name exists for " + dlcId);
		return null;
	}

	public static bool IsExpansion1Installed()
	{
		return false;
	}

	public static bool IsContentInstalled(string dlcId)
	{
		if (dlcId != null && (dlcId == null || dlcId.Length != 0))
		{
			if (dlcId == "EXPANSION1_ID")
			{
				return IsExpansion1Installed();
			}
			DebugUtil.LogErrorArgs("Invalid dlcId:", dlcId);
			return false;
		}
		return true;
	}

	public static void SetExpansion1Active(bool active)
	{
		Debug.Assert(condition: false, "Do not call this in the trunk version of the game!");
	}

	public static bool IsExpansion1Active()
	{
		return IsContentActive("EXPANSION1_ID");
	}

	public static bool IsContentActive(string dlcId)
	{
		return !IsAheadOf(dlcId, GetActiveDlcId());
	}

	public static string GetActiveDlcId()
	{
		if (IsExpansion1Installed() && IsContentSettingEnabled("EXPANSION1_ID"))
		{
			return "EXPANSION1_ID";
		}
		return "";
	}

	private static string GetInstalledDlcId()
	{
		if (IsExpansion1Installed())
		{
			return "EXPANSION1_ID";
		}
		return "";
	}

	private static bool IsAheadOf(string dlcIdA, string dlcIdB)
	{
		if (dlcIdA == null)
		{
			dlcIdA = "";
		}
		if (dlcIdB == null)
		{
			dlcIdB = "";
		}
		int num = RELEASE_ORDER.IndexOf(dlcIdA);
		Debug.Assert(num != -1, $"Invalid dlcIdA: {dlcIdA}");
		int num2 = RELEASE_ORDER.IndexOf(dlcIdB);
		Debug.Assert(num2 != -1, $"Invalid dlcIdB: {dlcIdB}");
		return num > num2;
	}

	private static bool IsContentSettingEnabled(string dlcId)
	{
		Debug.Assert(dlcId != "", "There is no KPlayerPrefs value for vanilla - it is always enabled");
		return false;
	}

	private static void SetContentSettingEnabled(string dlcId, bool enabled)
	{
		Debug.Assert(condition: false, "Don't call this in the trunk version of the game!");
	}
}
