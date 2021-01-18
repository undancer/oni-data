using System;
using System.Collections.Generic;

public class SandboxSettings
{
	public class Setting<T>
	{
		private string prefsKey;

		private Action<T> SetAction;

		public T defaultValue;

		public string PrefsKey => prefsKey;

		public T Value
		{
			set
			{
				SetAction(value);
			}
		}

		public Setting(string prefsKey, Action<T> setAction, T defaultValue)
		{
			this.prefsKey = prefsKey;
			SetAction = setAction;
			this.defaultValue = defaultValue;
		}
	}

	private List<Setting<int>> intSettings = new List<Setting<int>>();

	private List<Setting<float>> floatSettings = new List<Setting<float>>();

	private List<Setting<string>> stringSettings = new List<Setting<string>>();

	public bool InstantBuild = true;

	private bool hasRestoredElement;

	public Action<bool> OnChangeElement;

	public System.Action OnChangeMass;

	public System.Action OnChangeDisease;

	public System.Action OnChangeDiseaseCount;

	public System.Action OnChangeEntity;

	public System.Action OnChangeBrushSize;

	public System.Action OnChangeNoiseScale;

	public System.Action OnChangeNoiseDensity;

	public System.Action OnChangeTemperature;

	public System.Action OnChangeAdditiveTemperature;

	public const string KEY_SELECTED_ENTITY = "SandboxTools.SelectedEntity";

	public const string KEY_SELECTED_ELEMENT = "SandboxTools.SelectedElement";

	public const string KEY_SELECTED_DISEASE = "SandboxTools.SelectedDisease";

	public const string KEY_DISEASE_COUNT = "SandboxTools.DiseaseCount";

	public const string KEY_BRUSH_SIZE = "SandboxTools.BrushSize";

	public const string KEY_NOISE_SCALE = "SandboxTools.NoiseScale";

	public const string KEY_NOISE_DENSITY = "SandboxTools.NoiseDensity";

	public const string KEY_MASS = "SandboxTools.Mass";

	public const string KEY_TEMPERATURE = "SandbosTools.Temperature";

	public const string KEY_TEMPERATURE_ADDITIVE = "SandbosTools.TemperatureAdditive";

	public void AddIntSetting(string prefsKey, Action<int> setAction, int defaultValue)
	{
		intSettings.Add(new Setting<int>(prefsKey, setAction, defaultValue));
	}

	public int GetIntSetting(string prefsKey)
	{
		return KPlayerPrefs.GetInt(prefsKey);
	}

	public void SetIntSetting(string prefsKey, int value)
	{
		Setting<int> setting = intSettings.Find((Setting<int> match) => match.PrefsKey == prefsKey);
		if (setting == null)
		{
			Debug.LogError("No intSetting named: " + prefsKey + " could be found amongst " + intSettings.Count + " int settings.");
		}
		setting.Value = value;
	}

	public void RestoreIntSetting(string prefsKey)
	{
		if (KPlayerPrefs.HasKey(prefsKey))
		{
			SetIntSetting(prefsKey, GetIntSetting(prefsKey));
		}
		else
		{
			ForceDefaultIntSetting(prefsKey);
		}
	}

	public void ForceDefaultIntSetting(string prefsKey)
	{
		SetIntSetting(prefsKey, intSettings.Find((Setting<int> match) => match.PrefsKey == prefsKey).defaultValue);
	}

	public void AddFloatSetting(string prefsKey, Action<float> setAction, float defaultValue)
	{
		floatSettings.Add(new Setting<float>(prefsKey, setAction, defaultValue));
	}

	public float GetFloatSetting(string prefsKey)
	{
		return KPlayerPrefs.GetFloat(prefsKey);
	}

	public void SetFloatSetting(string prefsKey, float value)
	{
		Setting<float> setting = floatSettings.Find((Setting<float> match) => match.PrefsKey == prefsKey);
		if (setting == null)
		{
			Debug.LogError("No KPlayerPrefs float setting named: " + prefsKey + " could be found amongst " + floatSettings.Count + " float settings.");
		}
		setting.Value = value;
	}

	public void RestoreFloatSetting(string prefsKey)
	{
		if (KPlayerPrefs.HasKey(prefsKey))
		{
			SetFloatSetting(prefsKey, GetFloatSetting(prefsKey));
		}
		else
		{
			ForceDefaultFloatSetting(prefsKey);
		}
	}

	public void ForceDefaultFloatSetting(string prefsKey)
	{
		SetFloatSetting(prefsKey, floatSettings.Find((Setting<float> match) => match.PrefsKey == prefsKey).defaultValue);
	}

	public void AddStringSetting(string prefsKey, Action<string> setAction, string defaultValue)
	{
		stringSettings.Add(new Setting<string>(prefsKey, setAction, defaultValue));
	}

	public string GetStringSetting(string prefsKey)
	{
		return KPlayerPrefs.GetString(prefsKey);
	}

	public void SetStringSetting(string prefsKey, string value)
	{
		Setting<string> setting = stringSettings.Find((Setting<string> match) => match.PrefsKey == prefsKey);
		if (setting == null)
		{
			Debug.LogError("No KPlayerPrefs string setting named: " + prefsKey + " could be found amongst " + stringSettings.Count + " settings.");
		}
		setting.Value = value;
	}

	public void RestoreStringSetting(string prefsKey)
	{
		if (KPlayerPrefs.HasKey(prefsKey))
		{
			SetStringSetting(prefsKey, GetStringSetting(prefsKey));
		}
		else
		{
			ForceDefaultStringSetting(prefsKey);
		}
	}

	public void ForceDefaultStringSetting(string prefsKey)
	{
		SetStringSetting(prefsKey, stringSettings.Find((Setting<string> match) => match.PrefsKey == prefsKey).defaultValue);
	}

	public SandboxSettings()
	{
		AddStringSetting("SandboxTools.SelectedEntity", delegate(string data)
		{
			KPlayerPrefs.SetString("SandboxTools.SelectedEntity", data);
			OnChangeEntity();
		}, "MushBar");
		AddIntSetting("SandboxTools.SelectedElement", delegate(int data)
		{
			KPlayerPrefs.SetInt("SandboxTools.SelectedElement", data);
			OnChangeElement(hasRestoredElement);
			hasRestoredElement = true;
		}, ElementLoader.GetElementIndex(SimHashes.Oxygen));
		AddStringSetting("SandboxTools.SelectedDisease", delegate(string data)
		{
			KPlayerPrefs.SetString("SandboxTools.SelectedDisease", data);
			OnChangeDisease();
		}, Db.Get().Diseases.FoodGerms.Id);
		AddIntSetting("SandboxTools.DiseaseCount", delegate(int val)
		{
			KPlayerPrefs.SetInt("SandboxTools.DiseaseCount", val);
			OnChangeDiseaseCount();
		}, 0);
		AddIntSetting("SandboxTools.BrushSize", delegate(int val)
		{
			KPlayerPrefs.SetInt("SandboxTools.BrushSize", val);
			OnChangeBrushSize();
		}, 1);
		AddFloatSetting("SandboxTools.NoiseScale", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandboxTools.NoiseScale", val);
			OnChangeNoiseScale();
		}, 1f);
		AddFloatSetting("SandboxTools.NoiseDensity", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandboxTools.NoiseDensity", val);
			OnChangeNoiseDensity();
		}, 1f);
		AddFloatSetting("SandboxTools.Mass", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandboxTools.Mass", val);
			OnChangeMass();
		}, 1f);
		AddFloatSetting("SandbosTools.Temperature", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandbosTools.Temperature", val);
			OnChangeTemperature();
		}, 300f);
		AddFloatSetting("SandbosTools.TemperatureAdditive", delegate(float val)
		{
			KPlayerPrefs.SetFloat("SandbosTools.TemperatureAdditive", val);
			OnChangeAdditiveTemperature();
		}, 5f);
	}

	public void RestorePrefs()
	{
		foreach (Setting<int> intSetting in intSettings)
		{
			RestoreIntSetting(intSetting.PrefsKey);
		}
		foreach (Setting<float> floatSetting in floatSettings)
		{
			RestoreFloatSetting(floatSetting.PrefsKey);
		}
		foreach (Setting<string> stringSetting in stringSettings)
		{
			RestoreStringSetting(stringSetting.PrefsKey);
		}
	}
}
