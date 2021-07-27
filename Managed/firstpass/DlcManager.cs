using System;
using System.Collections.Generic;
using UnityEngine;

public class DlcManager
{
	[ThreadStatic]
	public static readonly bool IsMainThread = true;

	public const string VANILLA_ID = "";

	public const string EXPANSION1_ID = "EXPANSION1_ID";

	public const string VANILLA_DIRECTORY = "";

	public const string EXPANSION1_DIRECTORY = "expansion1";

	public static readonly string[] AVAILABLE_VANILLA_ONLY = new string[1] { "" };

	public static readonly string[] AVAILABLE_EXPANSION1_ONLY = new string[1] { "EXPANSION1_ID" };

	public static readonly string[] AVAILABLE_ALL_VERSIONS = new string[2] { "", "EXPANSION1_ID" };

	public static List<string> RELEASE_ORDER = new List<string> { "", "EXPANSION1_ID" };

	public static bool IsVanillaId(string dlcId)
	{
		if (dlcId != null)
		{
			return dlcId == "";
		}
		return true;
	}

	public static bool IsVanillaId(string[] dlcIds)
	{
		if (dlcIds != null)
		{
			if (dlcIds.Length == 1)
			{
				return dlcIds[0] == "";
			}
			return false;
		}
		return true;
	}

	public static bool IsValidForVanilla(string[] dlcIds)
	{
		if (dlcIds != null)
		{
			return Array.IndexOf(dlcIds, "") != -1;
		}
		return true;
	}

	public static bool IsExpansion1Id(string dlcId)
	{
		return dlcId == "EXPANSION1_ID";
	}

	public static string GetContentBundleName(string dlcId)
	{
		if (dlcId != null && dlcId == "EXPANSION1_ID")
		{
			return "expansion1_bundle";
		}
		Debug.LogError("No bundle exists for " + dlcId);
		return null;
	}

	public static string GetContentDirectoryName(string dlcId)
	{
		if (dlcId != null)
		{
			if (dlcId != null && dlcId.Length == 0)
			{
				return "";
			}
			if (dlcId == "EXPANSION1_ID")
			{
				return "expansion1";
			}
		}
		Debug.LogError("No content directory name exists for " + dlcId);
		return null;
	}

	public static string GetDlcIdFromContentDirectory(string contentDirectory)
	{
		if (contentDirectory != null)
		{
			if (contentDirectory != null && contentDirectory.Length == 0)
			{
				return "";
			}
			if (contentDirectory == "expansion1")
			{
				return "EXPANSION1_ID";
			}
		}
		Debug.LogError("No dlcId matches content directory " + contentDirectory);
		return null;
	}

	public static void ToggleDLC(string id)
	{
		SetContentSettingEnabled(id, !IsContentSettingEnabled(id));
	}

	public static bool IsContentActive(string dlcId)
	{
		if (CheckPlatformSubscription(dlcId))
		{
			return IsContentSettingEnabled(dlcId);
		}
		return false;
	}

	public static bool IsDlcListValidForCurrentContent(string[] dlcIds)
	{
		if (GetHighestActiveDlcId() == "")
		{
			return Array.IndexOf(dlcIds, "") != -1;
		}
		foreach (string text in dlcIds)
		{
			if (!(text == "") && IsContentActive(text))
			{
				return true;
			}
		}
		return false;
	}

	public static string GetHighestActiveDlcId()
	{
		for (int num = RELEASE_ORDER.Count - 1; num >= 0; num--)
		{
			string text = RELEASE_ORDER[num];
			if (CheckPlatformSubscription(text) && IsContentSettingEnabled(text))
			{
				return text;
			}
		}
		return "";
	}

	private static string GetInstalledDlcId()
	{
		if (CheckPlatformSubscription("EXPANSION1_ID"))
		{
			return "EXPANSION1_ID";
		}
		return "";
	}

	private static bool CheckPlatformSubscription(string dlcId)
	{
		if (dlcId == null || dlcId == "")
		{
			return true;
		}
		if (Application.isEditor && (!IsMainThread || !Application.isPlaying))
		{
			return true;
		}
		return DistributionPlatform.Inst.IsDLCSubscribed(dlcId);
	}

	private static bool IsContentSettingEnabled(string dlcId)
	{
		if (dlcId == null || dlcId == "")
		{
			return true;
		}
		if (!CheckPlatformSubscription(dlcId))
		{
			return false;
		}
		return KPlayerPrefs.GetInt(dlcId + ".ENABLED", 1) == 1;
	}

	private static bool IsContentOwned(string dlcId)
	{
		if (IsVanillaId(dlcId))
		{
			return true;
		}
		return DistributionPlatform.Inst.IsDLCPurchased(dlcId);
	}

	public static List<string> GetOwnedDLCIds()
	{
		List<string> list = new List<string>();
		for (int num = RELEASE_ORDER.Count - 1; num >= 0; num--)
		{
			string text = RELEASE_ORDER[num];
			if (!IsVanillaId(text) && IsContentOwned(text))
			{
				list.Add(text);
			}
		}
		return list;
	}

	public static List<string> GetActiveDLCIds()
	{
		List<string> list = new List<string>();
		for (int num = RELEASE_ORDER.Count - 1; num >= 0; num--)
		{
			string text = RELEASE_ORDER[num];
			if (!IsVanillaId(text) && IsContentActive(text))
			{
				list.Add(text);
			}
		}
		return list;
	}

	public static string GetContentLetter(string dlcId)
	{
		if (dlcId != null)
		{
			if (dlcId != null && dlcId.Length == 0)
			{
				return "B";
			}
			if (dlcId == "EXPANSION1_ID")
			{
				return "S";
			}
		}
		Debug.LogError("No content letter exists for " + dlcId);
		return null;
	}

	public static string GetActiveContentLetters()
	{
		if (IsPureVanilla())
		{
			return GetContentLetter("");
		}
		string text = "";
		for (int i = 0; i < RELEASE_ORDER.Count; i++)
		{
			string dlcId = RELEASE_ORDER[i];
			if (!IsVanillaId(dlcId) && IsContentActive(dlcId))
			{
				text += GetContentLetter(dlcId);
			}
		}
		return text;
	}

	private static void SetContentSettingEnabled(string dlcId, bool enabled)
	{
		Debug.Assert(dlcId != "", "There is no KPlayerPrefs value for vanilla - it is always enabled");
		bool flag = Application.isEditor || DistributionPlatform.Inst.IsDLCPurchased(dlcId);
		if (!enabled || flag)
		{
			KPlayerPrefs.SetInt(dlcId + ".ENABLED", enabled ? 1 : 0);
			if (enabled && !CheckPlatformSubscription(dlcId))
			{
				DistributionPlatform.Inst.ToggleDLCSubscription(dlcId);
			}
			else if ((bool)App.instance)
			{
				App.instance.Restart();
			}
		}
	}

	public static bool IsPureVanilla()
	{
		return !IsExpansion1Active();
	}

	public static bool IsExpansion1Installed()
	{
		return CheckPlatformSubscription("EXPANSION1_ID");
	}

	public static bool IsExpansion1Active()
	{
		return IsContentActive("EXPANSION1_ID");
	}

	public static bool FeatureRadiationEnabled()
	{
		return IsExpansion1Active();
	}

	public static bool FeaturePlantMutationsEnabled()
	{
		return IsExpansion1Active();
	}

	public static bool FeatureClusterSpaceEnabled()
	{
		return IsExpansion1Active();
	}
}
