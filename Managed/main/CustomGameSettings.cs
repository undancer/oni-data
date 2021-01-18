using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Klei;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/CustomGameSettings")]
public class CustomGameSettings : KMonoBehaviour
{
	public enum CustomGameMode
	{
		Survival = 0,
		Nosweat = 1,
		Custom = 0xFF
	}

	public struct MetricSettingsData
	{
		public string Name;

		public string Value;
	}

	private static CustomGameSettings instance;

	[Serialize]
	public bool is_custom_game;

	[Serialize]
	public CustomGameMode customGameMode;

	[Serialize]
	private Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();

	public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();

	private string hexChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	public static CustomGameSettings Instance => instance;

	public event Action<SettingConfig, SettingLevel> OnSettingChanged;

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 6))
		{
			customGameMode = (is_custom_game ? CustomGameMode.Custom : CustomGameMode.Survival);
		}
		if (CurrentQualityLevelsBySetting.ContainsKey("CarePackages "))
		{
			if (!CurrentQualityLevelsBySetting.ContainsKey(CustomGameSettingConfigs.CarePackages.id))
			{
				CurrentQualityLevelsBySetting.Add(CustomGameSettingConfigs.CarePackages.id, CurrentQualityLevelsBySetting["CarePackages "]);
			}
			CurrentQualityLevelsBySetting.Remove("CarePackages ");
		}
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		AddSettingConfig(CustomGameSettingConfigs.World);
		AddSettingConfig(CustomGameSettingConfigs.WorldgenSeed);
		AddSettingConfig(CustomGameSettingConfigs.ImmuneSystem);
		AddSettingConfig(CustomGameSettingConfigs.CalorieBurn);
		AddSettingConfig(CustomGameSettingConfigs.Morale);
		AddSettingConfig(CustomGameSettingConfigs.Stress);
		AddSettingConfig(CustomGameSettingConfigs.StressBreaks);
		AddSettingConfig(CustomGameSettingConfigs.CarePackages);
		AddSettingConfig(CustomGameSettingConfigs.SandboxMode);
		AddSettingConfig(CustomGameSettingConfigs.FastWorkersMode);
		if (SaveLoader.GetCloudSavesAvailable())
		{
			AddSettingConfig(CustomGameSettingConfigs.SaveToCloud);
		}
		VerifySettingCoordinates();
	}

	public void SetSurvivalDefaults()
	{
		customGameMode = CustomGameMode.Survival;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			SetQualitySetting(qualitySetting.Value, qualitySetting.Value.default_level_id);
		}
	}

	public void SetNosweatDefaults()
	{
		customGameMode = CustomGameMode.Nosweat;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			SetQualitySetting(qualitySetting.Value, qualitySetting.Value.nosweat_default_level_id);
		}
	}

	public SettingLevel CycleSettingLevel(ListSettingConfig config, int direction)
	{
		SetQualitySetting(config, config.CycleSettingLevelID(CurrentQualityLevelsBySetting[config.id], direction));
		return config.GetLevel(CurrentQualityLevelsBySetting[config.id]);
	}

	public SettingLevel ToggleSettingLevel(ToggleSettingConfig config)
	{
		SetQualitySetting(config, config.ToggleSettingLevelID(CurrentQualityLevelsBySetting[config.id]));
		return config.GetLevel(CurrentQualityLevelsBySetting[config.id]);
	}

	public void SetQualitySetting(SettingConfig config, string value)
	{
		CurrentQualityLevelsBySetting[config.id] = value;
		bool flag = true;
		bool flag2 = true;
		foreach (KeyValuePair<string, string> item in CurrentQualityLevelsBySetting)
		{
			if (!QualitySettings.ContainsKey(item.Key))
			{
				DebugUtil.LogWarningArgs("Quality settings missing " + item.Key);
			}
			else if (QualitySettings[item.Key].triggers_custom_game)
			{
				if (item.Value != QualitySettings[item.Key].default_level_id)
				{
					flag = false;
				}
				if (item.Value != QualitySettings[item.Key].nosweat_default_level_id)
				{
					flag2 = false;
				}
				if (!flag && !flag2)
				{
					break;
				}
			}
		}
		CustomGameMode customGameMode = ((!flag) ? (flag2 ? CustomGameMode.Nosweat : CustomGameMode.Custom) : CustomGameMode.Survival);
		if (customGameMode != this.customGameMode)
		{
			DebugUtil.LogArgs("Game mode changed from", this.customGameMode, "to", customGameMode);
			this.customGameMode = customGameMode;
		}
		if (this.OnSettingChanged != null)
		{
			this.OnSettingChanged(config, GetCurrentQualitySetting(config));
		}
	}

	public SettingLevel GetCurrentQualitySetting(SettingConfig setting)
	{
		return GetCurrentQualitySetting(setting.id);
	}

	public SettingLevel GetCurrentQualitySetting(string setting_id)
	{
		SettingConfig settingConfig = QualitySettings[setting_id];
		if (customGameMode == CustomGameMode.Survival && settingConfig.triggers_custom_game)
		{
			return settingConfig.GetLevel(settingConfig.default_level_id);
		}
		if (customGameMode == CustomGameMode.Nosweat && settingConfig.triggers_custom_game)
		{
			return settingConfig.GetLevel(settingConfig.nosweat_default_level_id);
		}
		if (!CurrentQualityLevelsBySetting.ContainsKey(setting_id))
		{
			CurrentQualityLevelsBySetting[setting_id] = QualitySettings[setting_id].default_level_id;
		}
		string level_id = CurrentQualityLevelsBySetting[setting_id];
		return QualitySettings[setting_id].GetLevel(level_id);
	}

	public string GetCurrentQualitySettingLevelId(SettingConfig config)
	{
		return CurrentQualityLevelsBySetting[config.id];
	}

	public string GetSettingLevelLabel(string setting_id, string level_id)
	{
		SettingConfig settingConfig = QualitySettings[setting_id];
		if (settingConfig != null)
		{
			SettingLevel level = settingConfig.GetLevel(level_id);
			if (level != null)
			{
				return level.label;
			}
		}
		Debug.LogWarning("No label string for setting: " + setting_id + " level: " + level_id);
		return "";
	}

	public string GetSettingLevelTooltip(string setting_id, string level_id)
	{
		SettingConfig settingConfig = QualitySettings[setting_id];
		if (settingConfig != null)
		{
			SettingLevel level = settingConfig.GetLevel(level_id);
			if (level != null)
			{
				return level.tooltip;
			}
		}
		Debug.LogWarning("No tooltip string for setting: " + setting_id + " level: " + level_id);
		return "";
	}

	public void AddSettingConfig(SettingConfig config)
	{
		QualitySettings.Add(config.id, config);
		if (!CurrentQualityLevelsBySetting.ContainsKey(config.id) || string.IsNullOrEmpty(CurrentQualityLevelsBySetting[config.id]))
		{
			CurrentQualityLevelsBySetting[config.id] = config.default_level_id;
		}
	}

	private static void AddWorldMods(object user_data, List<SettingLevel> levels)
	{
		string path = FileSystem.Normalize(System.IO.Path.Combine(SettingsCache.GetPath(), "worlds"));
		ListPool<string, CustomGameSettings>.PooledList pooledList = ListPool<string, CustomGameSettings>.Allocate();
		FileSystem.GetFiles(path, "*.yaml", pooledList);
		foreach (string item in pooledList)
		{
			ProcGen.World world = YamlIO.LoadFile<ProcGen.World>(item);
			string worldName = Worlds.GetWorldName(item);
			levels.Add(new SettingLevel(worldName, world.name, world.description, 0, user_data));
		}
		pooledList.Recycle();
	}

	public void LoadWorlds()
	{
		Dictionary<string, ProcGen.World> worldCache = SettingsCache.worlds.worldCache;
		List<SettingLevel> list = new List<SettingLevel>(worldCache.Count);
		foreach (KeyValuePair<string, ProcGen.World> item in worldCache)
		{
			StringEntry result;
			string label = (Strings.TryGet(new StringKey(item.Value.name), out result) ? result.ToString() : item.Value.name);
			string tooltip = (Strings.TryGet(new StringKey(item.Value.description), out result) ? result.ToString() : item.Value.description);
			list.Add(new SettingLevel(item.Key, label, tooltip));
		}
		CustomGameSettingConfigs.World.StompLevels(list, "worlds/SandstoneDefault", "worlds/SandstoneDefault");
	}

	public void Print()
	{
		string text = "Custom Settings: ";
		foreach (KeyValuePair<string, string> item in CurrentQualityLevelsBySetting)
		{
			text = text + item.Key + "=" + item.Value + ",";
		}
		Debug.Log(text);
	}

	private bool AllValuesMatch(Dictionary<string, string> data, CustomGameMode mode)
	{
		bool result = true;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			if (!(qualitySetting.Key == CustomGameSettingConfigs.WorldgenSeed.id))
			{
				string b = null;
				switch (mode)
				{
				case CustomGameMode.Nosweat:
					b = qualitySetting.Value.nosweat_default_level_id;
					break;
				case CustomGameMode.Survival:
					b = qualitySetting.Value.default_level_id;
					break;
				}
				if (data.ContainsKey(qualitySetting.Key) && data[qualitySetting.Key] != b)
				{
					return false;
				}
			}
		}
		return result;
	}

	public List<MetricSettingsData> GetSettingsForMetrics()
	{
		List<MetricSettingsData> list = new List<MetricSettingsData>();
		MetricSettingsData metricSettingsData = new MetricSettingsData
		{
			Name = "CustomGameMode",
			Value = this.customGameMode.ToString()
		};
		list.Add(metricSettingsData);
		foreach (KeyValuePair<string, string> item2 in CurrentQualityLevelsBySetting)
		{
			metricSettingsData = new MetricSettingsData
			{
				Name = item2.Key,
				Value = item2.Value
			};
			list.Add(metricSettingsData);
		}
		metricSettingsData = default(MetricSettingsData);
		metricSettingsData.Name = "CustomGameModeActual";
		metricSettingsData.Value = CustomGameMode.Custom.ToString();
		MetricSettingsData item = metricSettingsData;
		foreach (CustomGameMode value in Enum.GetValues(typeof(CustomGameMode)))
		{
			if (value != CustomGameMode.Custom && AllValuesMatch(CurrentQualityLevelsBySetting, value))
			{
				item.Value = value.ToString();
				break;
			}
		}
		list.Add(item);
		return list;
	}

	public bool VerifySettingCoordinates()
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		bool result = false;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			if (qualitySetting.Value.coordinate_dimension < 0 || qualitySetting.Value.coordinate_dimension_width < 0)
			{
				if (qualitySetting.Value.coordinate_dimension >= 0 || qualitySetting.Value.coordinate_dimension_width >= 0)
				{
					result = true;
					Debug.Assert(condition: false, qualitySetting.Value.id + ": Both coordinate dimension props must be unset (-1) if either is unset.");
				}
				continue;
			}
			List<SettingLevel> levels = qualitySetting.Value.GetLevels();
			if (qualitySetting.Value.coordinate_dimension_width < levels.Count)
			{
				result = true;
				Debug.Assert(condition: false, qualitySetting.Value.id + ": Range between coordinate min and max insufficient for all levels (" + qualitySetting.Value.coordinate_dimension_width + "<" + levels.Count + ")");
			}
			foreach (SettingLevel item in levels)
			{
				int key = qualitySetting.Value.coordinate_dimension * item.coordinate_offset;
				string text = qualitySetting.Value.id + " > " + item.id;
				if (item.coordinate_offset < 0)
				{
					result = true;
					Debug.Assert(condition: false, text + ": Level coordinate offset must be >= 0");
					continue;
				}
				if (item.coordinate_offset == 0)
				{
					if (item.id != qualitySetting.Value.default_level_id)
					{
						result = true;
						Debug.Assert(condition: false, text + ": Only the default level should have a coordinate offset of 0");
					}
					continue;
				}
				if (item.coordinate_offset > qualitySetting.Value.coordinate_dimension_width)
				{
					result = true;
					Debug.Assert(condition: false, text + ": level coordinate must be <= dimension width");
					continue;
				}
				string value;
				bool flag = !dictionary.TryGetValue(key, out value);
				dictionary[key] = text;
				if (item.id == qualitySetting.Value.default_level_id)
				{
					result = true;
					Debug.Assert(condition: false, text + ": Default level must be coordinate 0");
				}
				if (!flag)
				{
					result = true;
					Debug.Assert(condition: false, text + ": Combined coordinate conflicts with another coordinate (" + value + "). Ensure this SettingConfig's min and max don't overlap with another SettingConfig's");
				}
			}
		}
		return result;
	}

	public string[] ParseSettingCoordinate(string coord)
	{
		Match match = new Regex("(.*)-(.*)-(.*)").Match(coord);
		string[] array = new string[match.Groups.Count];
		for (int i = 0; i < match.Groups.Count; i++)
		{
			array[i] = match.Groups[i].Value;
		}
		return array;
	}

	public string GetSettingsCoordinate()
	{
		ProcGen.World worldData = SettingsCache.worlds.GetWorldData(Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id);
		SettingLevel currentQualitySetting = Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		string otherSettingsCode = GetOtherSettingsCode();
		return $"{worldData.GetCoordinatePrefix()}-{currentQualitySetting.id}-{otherSettingsCode}";
	}

	public void ParseAndApplySettingsCode(string code)
	{
		int num = Base36toBase10(code);
		Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
		foreach (KeyValuePair<string, string> item in CurrentQualityLevelsBySetting)
		{
			SettingConfig settingConfig = QualitySettings[item.Key];
			if (settingConfig.coordinate_dimension < 0 || settingConfig.coordinate_dimension_width < 0)
			{
				continue;
			}
			int num2 = 0;
			int num3 = settingConfig.coordinate_dimension * settingConfig.coordinate_dimension_width;
			int num4 = num;
			if (num4 >= num3)
			{
				int num5 = num4 / num3 * num3;
				num4 -= num5;
			}
			if (num4 >= settingConfig.coordinate_dimension)
			{
				num2 = num4 / settingConfig.coordinate_dimension;
			}
			foreach (SettingLevel level in settingConfig.GetLevels())
			{
				if (level.coordinate_offset == num2)
				{
					dictionary[settingConfig] = level.id;
					break;
				}
			}
		}
		foreach (KeyValuePair<SettingConfig, string> item2 in dictionary)
		{
			SetQualitySetting(item2.Key, item2.Value);
		}
	}

	private string GetOtherSettingsCode()
	{
		int num = 0;
		foreach (KeyValuePair<string, string> item in CurrentQualityLevelsBySetting)
		{
			QualitySettings.TryGetValue(item.Key, out var value);
			if (value != null && value.coordinate_dimension >= 0 && value.coordinate_dimension_width >= 0)
			{
				SettingLevel level = value.GetLevel(item.Value);
				int num2 = value.coordinate_dimension * level.coordinate_offset;
				num += num2;
			}
		}
		return Base10toBase36(num);
	}

	private int Base36toBase10(string input)
	{
		if (input == "0")
		{
			return 0;
		}
		int num = 0;
		for (int num2 = input.Length - 1; num2 >= 0; num2--)
		{
			num *= 36;
			int num3 = hexChars.IndexOf(input[num2]);
			num += num3;
		}
		return num;
	}

	private string Base10toBase36(int input)
	{
		if (input == 0)
		{
			return "0";
		}
		int num = input;
		string text = "";
		while (num > 0)
		{
			text += hexChars[num % 36];
			num /= 36;
		}
		return text;
	}
}
