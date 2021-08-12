using System;
using System.Collections.Generic;
using System.IO;
using Klei;

public class KPlayerPrefs
{
	private static KPlayerPrefs _instance = null;

	private static bool _corruptedFlag = false;

	public const string KPLAYER_PREFS_DATA_COLLECTION_KEY = "DisableDataCollection";

	public static readonly string FILENAME = "kplayerprefs.yaml";

	private static string PATH = null;

	public static KPlayerPrefs instance
	{
		get
		{
			if (_instance == null)
			{
				try
				{
					_instance = new KPlayerPrefs();
					PATH = GetPath();
					_instance = YamlIO.LoadFile<KPlayerPrefs>(PATH);
				}
				catch
				{
				}
			}
			if (_instance == null)
			{
				Debug.LogWarning("Failed to load KPlayerPrefs, Creating new instance..");
				_corruptedFlag = true;
				_instance = new KPlayerPrefs();
			}
			return _instance;
		}
	}

	public Dictionary<string, string> strings { get; private set; }

	public Dictionary<string, int> ints { get; private set; }

	public Dictionary<string, float> floats { get; private set; }

	public KPlayerPrefs()
	{
		strings = new Dictionary<string, string>();
		ints = new Dictionary<string, int>();
		floats = new Dictionary<string, float>();
		_instance = this;
	}

	public static bool HasCorruptedFlag()
	{
		return _corruptedFlag;
	}

	public static void ResetCorruptedFlag()
	{
		_corruptedFlag = false;
	}

	public static void DeleteAll()
	{
		instance.strings.Clear();
		instance.ints.Clear();
		instance.floats.Clear();
		Save();
	}

	private static string GetPath()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		return Path.Combine(Util.RootFolder(), FILENAME);
	}

	public static void Save()
	{
		try
		{
			YamlIO.SaveOrWarnUser(instance, PATH);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to save kplayerprefs: " + ex.ToString());
		}
	}

	public void Load()
	{
	}

	public static void DeleteKey(string key)
	{
		instance.strings.Remove(key);
		instance.ints.Remove(key);
		instance.floats.Remove(key);
	}

	public static float GetFloat(string key)
	{
		float value = 0f;
		instance.floats.TryGetValue(key, out value);
		return value;
	}

	public static float GetFloat(string key, float defaultValue)
	{
		float value = 0f;
		if (!instance.floats.TryGetValue(key, out value))
		{
			value = defaultValue;
		}
		return value;
	}

	public static int GetInt(string key)
	{
		int value = 0;
		instance.ints.TryGetValue(key, out value);
		return value;
	}

	public static int GetInt(string key, int defaultValue)
	{
		int value = 0;
		if (!instance.ints.TryGetValue(key, out value))
		{
			value = defaultValue;
		}
		return value;
	}

	public static string GetString(string key)
	{
		string value = null;
		instance.strings.TryGetValue(key, out value);
		return value;
	}

	public static string GetString(string key, string defaultValue)
	{
		string value = null;
		if (!instance.strings.TryGetValue(key, out value))
		{
			value = defaultValue;
		}
		return value;
	}

	public static bool HasKey(string key)
	{
		if (instance.strings.ContainsKey(key))
		{
			return true;
		}
		if (instance.ints.ContainsKey(key))
		{
			return true;
		}
		if (instance.floats.ContainsKey(key))
		{
			return true;
		}
		return false;
	}

	public static void SetFloat(string key, float value)
	{
		if (instance.floats.ContainsKey(key))
		{
			instance.floats[key] = value;
		}
		else
		{
			instance.floats.Add(key, value);
		}
		Save();
	}

	public static void SetInt(string key, int value)
	{
		if (instance.ints.ContainsKey(key))
		{
			instance.ints[key] = value;
		}
		else
		{
			instance.ints.Add(key, value);
		}
		Save();
	}

	public static void SetString(string key, string value)
	{
		if (instance.strings.ContainsKey(key))
		{
			instance.strings[key] = value;
		}
		else
		{
			instance.strings.Add(key, value);
		}
		Save();
	}
}
