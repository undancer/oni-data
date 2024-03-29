using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Klei;
using Newtonsoft.Json;
using UnityEngine;

public class TuningSystem
{
	private static JsonSerializerSettings _SerializationSettings;

	private static Dictionary<Type, object> _TuningValues;

	private static string _TuningPath;

	public static void Init()
	{
	}

	static TuningSystem()
	{
		_SerializationSettings = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented
		};
		_TuningPath = Path.Combine(Application.streamingAssetsPath, "Tuning.json");
		InitializeTuning();
		ListenForFileChanges();
		Load();
	}

	private static void ListenForFileChanges()
	{
		string directoryName = Path.GetDirectoryName(_TuningPath);
		try
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
			fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
			fileSystemWatcher.Changed += OnFileChanged;
			fileSystemWatcher.Path = directoryName;
			fileSystemWatcher.Filter = Path.GetFileName(_TuningPath);
			fileSystemWatcher.EnableRaisingEvents = true;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Error when attempting to monitor path: " + directoryName + "\n" + ex.ToString());
		}
	}

	private static void OnFileChanged(object source, FileSystemEventArgs e)
	{
		Load();
	}

	private static void InitializeTuning()
	{
		_TuningValues = new Dictionary<Type, object>();
		foreach (Type currentDomainType in App.GetCurrentDomainTypes())
		{
			Type baseType = currentDomainType.BaseType;
			if (!(baseType == null) && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(TuningData<>))
			{
				object obj = Activator.CreateInstance(baseType.GetGenericArguments()[0]);
				_TuningValues[obj.GetType()] = obj;
				baseType.GetField("_TuningData", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).SetValue(null, obj);
			}
		}
	}

	private static void Save()
	{
		if (_TuningValues == null)
		{
			return;
		}
		JsonSerializer jsonSerializer = JsonSerializer.Create(_SerializationSettings);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<Type, object> tuningValue in _TuningValues)
		{
			dictionary[tuningValue.Value.GetType().FullName] = tuningValue.Value;
		}
		using StreamWriter textWriter = File.CreateText(_TuningPath);
		jsonSerializer.Serialize(textWriter, dictionary);
	}

	private static void Load()
	{
		string[] array = new string[2]
		{
			_TuningPath,
			string.Empty
		};
		if (Thread.CurrentThread == KProfiler.main_thread)
		{
			array[1] = Path.Combine(Application.dataPath, "Tuning.json");
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (string.IsNullOrEmpty(text) || !FileSystem.FileExists(text))
			{
				continue;
			}
			foreach (KeyValuePair<string, object> item in JsonConvert.DeserializeObject<Dictionary<string, object>>(FileSystem.ConvertToText(FileSystem.ReadBytes(text))))
			{
				Type type = null;
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int j = 0; j < assemblies.Length; j++)
				{
					type = assemblies[j].GetType(item.Key);
					if (type != null)
					{
						break;
					}
				}
				if (type != null)
				{
					if (_TuningValues.ContainsKey(type))
					{
						JsonConvert.PopulateObject(item.Value.ToString(), _TuningValues[type]);
						continue;
					}
					object value = JsonConvert.DeserializeObject(item.Value.ToString(), type);
					_TuningValues[type] = value;
				}
			}
		}
	}

	private static bool IsLoaded()
	{
		return _TuningValues != null;
	}

	public static Dictionary<Type, object> GetAllTuningValues()
	{
		return _TuningValues;
	}
}
