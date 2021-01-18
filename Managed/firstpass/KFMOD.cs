using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KFMOD
{
	private struct SoundCountEntry
	{
		public int count;

		public float minObjects;

		public float maxObjects;
	}

	public struct AudioDevice
	{
		public int fmod_id;

		public string name;

		public Guid guid;

		public int systemRate;

		public SPEAKERMODE speakerMode;

		public int speakerModeChannels;

		public bool selected;
	}

	private static Dictionary<HashedString, SoundDescription> soundDescriptions = new Dictionary<HashedString, SoundDescription>();

	public static bool didFmodInitializeSuccessfully = true;

	private static Dictionary<HashedString, OneShotSoundParameterUpdater> parameterUpdaters = new Dictionary<HashedString, OneShotSoundParameterUpdater>();

	public static AudioDevice currentDevice;

	public static SoundDescription GetSoundEventDescription(HashedString path)
	{
		if (!soundDescriptions.ContainsKey(path))
		{
			return default(SoundDescription);
		}
		return soundDescriptions[path];
	}

	public static void Initialize()
	{
		try
		{
			FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
			didFmodInitializeSuccessfully = RuntimeManager.IsInitialized;
		}
		catch (Exception ex)
		{
			didFmodInitializeSuccessfully = false;
			if (!(ex.GetType() == typeof(SystemNotInitializedException)))
			{
				throw ex;
			}
			Debug.LogWarning(ex);
		}
		CollectParameterUpdaters();
		CollectSoundDescriptions();
	}

	public static void PlayOneShot(string sound, Vector3 position, float volume = 1f)
	{
		EventInstance instance = BeginOneShot(sound, position, volume);
		EndOneShot(instance);
	}

	public static void PlayUISound(string sound)
	{
		PlayOneShot(sound, Vector3.zero);
	}

	public static EventInstance BeginOneShot(string sound, Vector3 position, float volume = 1f)
	{
		if (string.IsNullOrEmpty(sound) || App.IsExiting || !RuntimeManager.IsInitialized)
		{
			return default(EventInstance);
		}
		EventInstance result = CreateInstance(sound);
		if (!result.isValid())
		{
			if (KFMODDebugger.instance != null)
			{
			}
			return result;
		}
		Vector3 pos = new Vector3(position.x, position.y, position.z);
		if (KFMODDebugger.instance != null)
		{
		}
		ATTRIBUTES_3D attributes = pos.To3DAttributes();
		result.set3DAttributes(attributes);
		result.setVolume(volume);
		return result;
	}

	public static bool EndOneShot(EventInstance instance)
	{
		if (!instance.isValid())
		{
			return false;
		}
		instance.start();
		instance.release();
		return true;
	}

	public static EventInstance CreateInstance(string path)
	{
		if (!RuntimeManager.IsInitialized)
		{
			return default(EventInstance);
		}
		EventInstance eventInstance;
		try
		{
			eventInstance = RuntimeManager.CreateInstance(path);
		}
		catch (EventNotFoundException obj)
		{
			Debug.LogWarning(obj);
			return default(EventInstance);
		}
		HashedString path2 = path;
		SoundDescription soundEventDescription = GetSoundEventDescription(path2);
		OneShotSoundParameterUpdater.Sound sound = default(OneShotSoundParameterUpdater.Sound);
		sound.ev = eventInstance;
		sound.path = path2;
		sound.description = soundEventDescription;
		OneShotSoundParameterUpdater.Sound sound2 = sound;
		OneShotSoundParameterUpdater[] oneShotParameterUpdaters = soundEventDescription.oneShotParameterUpdaters;
		foreach (OneShotSoundParameterUpdater oneShotSoundParameterUpdater in oneShotParameterUpdaters)
		{
			oneShotSoundParameterUpdater.Play(sound2);
		}
		return eventInstance;
	}

	private static void CollectSoundDescriptions()
	{
		Bank[] array = null;
		RuntimeManager.StudioSystem.getBankList(out array);
		Bank[] array2 = array;
		foreach (Bank bank in array2)
		{
			bank.getEventList(out var array3);
			for (int j = 0; j < array3.Length; j++)
			{
				EventDescription eventDescription = array3[j];
				eventDescription.getPath(out var path);
				HashedString key = path;
				SoundDescription value = default(SoundDescription);
				value.path = path;
				float distance = 0f;
				eventDescription.getMaximumDistance(out distance);
				if (distance == 0f)
				{
					distance = 60f;
				}
				value.falloffDistanceSq = distance * distance;
				List<OneShotSoundParameterUpdater> list = new List<OneShotSoundParameterUpdater>();
				int count = 0;
				eventDescription.getParameterDescriptionCount(out count);
				SoundDescription.Parameter[] array4 = new SoundDescription.Parameter[count];
				for (int k = 0; k < count; k++)
				{
					eventDescription.getParameterDescriptionByIndex(k, out var parameter);
					string text = parameter.name;
					array4[k] = new SoundDescription.Parameter
					{
						name = new HashedString(text),
						id = parameter.id
					};
					OneShotSoundParameterUpdater value2 = null;
					if (parameterUpdaters.TryGetValue(text, out value2))
					{
						list.Add(value2);
					}
				}
				value.parameters = array4;
				value.oneShotParameterUpdaters = list.ToArray();
				soundDescriptions[key] = value;
			}
		}
	}

	private static void CollectParameterUpdaters()
	{
		foreach (Type currentDomainType in App.GetCurrentDomainTypes())
		{
			if (currentDomainType.IsAbstract)
			{
				continue;
			}
			bool flag = false;
			Type baseType = currentDomainType.BaseType;
			while (baseType != null)
			{
				if (baseType == typeof(OneShotSoundParameterUpdater))
				{
					flag = true;
					break;
				}
				baseType = baseType.BaseType;
			}
			if (flag)
			{
				OneShotSoundParameterUpdater oneShotSoundParameterUpdater = (OneShotSoundParameterUpdater)Activator.CreateInstance(currentDomainType);
				DebugUtil.Assert(!parameterUpdaters.ContainsKey(oneShotSoundParameterUpdater.parameter));
				parameterUpdaters[oneShotSoundParameterUpdater.parameter] = oneShotSoundParameterUpdater;
			}
		}
	}

	public static void RenderEveryTick(float dt)
	{
		foreach (KeyValuePair<HashedString, OneShotSoundParameterUpdater> parameterUpdater in parameterUpdaters)
		{
			parameterUpdater.Value.Update(dt);
		}
	}
}
