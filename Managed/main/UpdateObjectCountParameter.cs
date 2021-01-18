using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

internal class UpdateObjectCountParameter : LoopingSoundParameterUpdater
{
	private struct Entry
	{
		public EventInstance ev;

		public Settings settings;
	}

	public struct Settings
	{
		public HashedString path;

		public PARAMETER_ID parameterId;

		public float minObjects;

		public float maxObjects;

		public bool useExponentialCurve;
	}

	private List<Entry> entries = new List<Entry>();

	private static Dictionary<HashedString, Settings> settings = new Dictionary<HashedString, Settings>();

	private static readonly HashedString parameterHash = "objectCount";

	public static Settings GetSettings(HashedString path_hash, SoundDescription description)
	{
		Settings value = default(Settings);
		if (!settings.TryGetValue(path_hash, out value))
		{
			value = default(Settings);
			EventDescription eventDescription = RuntimeManager.GetEventDescription(description.path);
			if (eventDescription.getUserProperty("minObj", out var property) == RESULT.OK)
			{
				value.minObjects = (short)property.floatValue();
			}
			else
			{
				value.minObjects = 1f;
			}
			if (eventDescription.getUserProperty("maxObj", out var property2) == RESULT.OK)
			{
				value.maxObjects = property2.floatValue();
			}
			else
			{
				value.maxObjects = 0f;
			}
			if (eventDescription.getUserProperty("curveType", out var property3) == RESULT.OK && property3.stringValue() == "exp")
			{
				value.useExponentialCurve = true;
			}
			value.parameterId = description.GetParameterId(parameterHash);
			value.path = path_hash;
			settings[path_hash] = value;
		}
		return value;
	}

	public static void ApplySettings(EventInstance ev, int count, Settings settings)
	{
		float num = 0f;
		if (settings.maxObjects != settings.minObjects)
		{
			num = ((float)count - settings.minObjects) / (settings.maxObjects - settings.minObjects);
			num = Mathf.Clamp01(num);
		}
		if (settings.useExponentialCurve)
		{
			num *= num;
		}
		ev.setParameterByID(settings.parameterId, num);
	}

	public UpdateObjectCountParameter()
		: base("objectCount")
	{
	}

	public override void Add(Sound sound)
	{
		Settings settings = GetSettings(sound.path, sound.description);
		Entry entry = default(Entry);
		entry.ev = sound.ev;
		entry.settings = settings;
		Entry item = entry;
		entries.Add(item);
	}

	public override void Update(float dt)
	{
		DictionaryPool<HashedString, int, LoopingSoundManager>.PooledDictionary pooledDictionary = DictionaryPool<HashedString, int, LoopingSoundManager>.Allocate();
		foreach (Entry entry in entries)
		{
			int value = 0;
			pooledDictionary.TryGetValue(entry.settings.path, out value);
			value = (pooledDictionary[entry.settings.path] = value + 1);
		}
		foreach (Entry entry2 in entries)
		{
			int count = pooledDictionary[entry2.settings.path];
			ApplySettings(entry2.ev, count, entry2.settings);
		}
		pooledDictionary.Recycle();
	}

	public override void Remove(Sound sound)
	{
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].ev.handle == sound.ev.handle)
			{
				entries.RemoveAt(i);
				break;
			}
		}
	}

	public static void Clear()
	{
		settings.Clear();
	}
}
