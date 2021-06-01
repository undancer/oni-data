using System.Collections.Generic;
using Klei;

public class DlcManager
{
	public const string VANILLA_ID = "";

	public const string EXPANSION1_ID = "EXPANSION1_ID";

	public const string VANILLA_DIRECTORY = "";

	public const string EXPANSION1_DIRECTORY = "expansion1";

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
		return dlcId == null || dlcId == "";
	}

	public static bool IsExpansion1Id(string dlcId)
	{
		return dlcId == "EXPANSION1_ID";
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
		if (dlcId == null || dlcId.Length != 0)
		{
			if (dlcId == "EXPANSION1_ID")
			{
				return "expansion1";
			}
			Debug.LogError("No content directory name exists for " + dlcId);
			return null;
		}
		return "";
	}

	public static string GetDlcIdFromContentDirectory(string contentDirectory)
	{
		if (contentDirectory == null || contentDirectory.Length != 0)
		{
			if (contentDirectory == "expansion1")
			{
				return "EXPANSION1_ID";
			}
			Debug.LogError("No dlcId matches content directory " + contentDirectory);
			return null;
		}
		return "";
	}

	public static bool IsExpansion1Installed()
	{
		if (GenericGameSettings.instance.disableExpansion1)
		{
			return false;
		}
		return true;
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
		Debug.Assert(!active || IsExpansion1Installed(), "Cannot set Expansion1 active if it isn't installed");
		SetContentSettingEnabled("EXPANSION1_ID", active);
	}

	public static void ToggleDLC()
	{
		if (DistributionPlatform.Inst.PurchasedDLC)
		{
			DistributionPlatform.Inst.ToggleDLC();
		}
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
		return KPlayerPrefs.GetInt(dlcId + ".ENABLED", 1) == 1;
	}

	private static void SetContentSettingEnabled(string dlcId, bool enabled)
	{
		Debug.Assert(dlcId != "", "There is no KPlayerPrefs value for vanilla - it is always enabled");
		KPlayerPrefs.SetInt(dlcId + ".ENABLED", enabled ? 1 : 0);
	}

	public static bool FeatureRadiationEnabled()
	{
		return IsExpansion1Active();
	}

	public static bool FeaturePlantMutationsEnabled()
	{
		return IsExpansion1Active();
	}
}
