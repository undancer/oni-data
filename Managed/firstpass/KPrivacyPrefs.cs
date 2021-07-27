using System;
using System.IO;
using Klei;

public class KPrivacyPrefs
{
	private static KPrivacyPrefs _instance;

	public static readonly string FILENAME = "kprivacyprefs.yaml";

	public bool disableDataCollection { get; set; }

	public static KPrivacyPrefs instance
	{
		get
		{
			if (_instance == null)
			{
				Load();
			}
			return _instance;
		}
	}

	public static string GetPath()
	{
		return Path.Combine(GetDirectory(), FILENAME);
	}

	public static string GetDirectory()
	{
		return Path.Combine(Path.Combine(Util.GetKleiRootPath(), "Agreements"), Util.GetTitleFolderName());
	}

	public static void Save()
	{
		try
		{
			if (!Directory.Exists(GetDirectory()))
			{
				Directory.CreateDirectory(GetDirectory());
			}
			YamlIO.SaveOrWarnUser(instance, GetPath());
		}
		catch (Exception ex)
		{
			LogError(ex.ToString());
		}
	}

	public static void Load()
	{
		try
		{
			if (_instance == null)
			{
				_instance = new KPrivacyPrefs();
			}
			string path = GetPath();
			if (File.Exists(path))
			{
				_instance = YamlIO.LoadFile<KPrivacyPrefs>(path);
				if (_instance == null)
				{
					LogError("Exception while loading privacy prefs:" + path);
					_instance = new KPrivacyPrefs();
				}
			}
		}
		catch (Exception ex)
		{
			LogError(ex.ToString());
		}
	}

	private static void LogError(string msg)
	{
		Debug.LogWarning(msg);
	}
}
