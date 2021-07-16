using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LoopingSoundManager")]
public class LoopingSoundManager : KMonoBehaviour, IRenderEveryTick
{
	public class Tuning : TuningData<Tuning>
	{
		public float velocityScale;
	}

	public struct Sound
	{
		[Flags]
		public enum Flags
		{
			PLAYING = 0x1,
			PAUSE_ON_GAME_PAUSED = 0x2,
			ENABLE_CULLING = 0x4,
			ENABLE_CAMERA_SCALED_POSITION = 0x8
		}

		public EventInstance ev;

		public Transform transform;

		public KBatchedAnimController animController;

		public float falloffDistanceSq;

		public HashedString path;

		public Vector3 pos;

		public Vector2 velocity;

		public HashedString firstParameter;

		public HashedString secondParameter;

		public float firstParameterValue;

		public float secondParameterValue;

		public float vol;

		public bool objectIsSelectedAndVisible;

		public Flags flags;

		public bool IsPlaying => (flags & Flags.PLAYING) != 0;

		public bool ShouldPauseOnGamePaused => (flags & Flags.PAUSE_ON_GAME_PAUSED) != 0;

		public bool IsCullingEnabled => (flags & Flags.ENABLE_CULLING) != 0;

		public bool ShouldCameraScalePosition => (flags & Flags.ENABLE_CAMERA_SCALED_POSITION) != 0;
	}

	private static LoopingSoundManager instance;

	private bool GameIsPaused;

	private Dictionary<HashedString, LoopingSoundParameterUpdater> parameterUpdaters = new Dictionary<HashedString, LoopingSoundParameterUpdater>();

	private KCompactedVector<Sound> sounds = new KCompactedVector<Sound>();

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		CollectParameterUpdaters();
	}

	protected override void OnSpawn()
	{
		if (SpeedControlScreen.Instance != null && Game.Instance != null)
		{
			Game.Instance.Subscribe(-1788536802, instance.OnPauseChanged);
		}
		Game.Instance.Subscribe(1983128072, delegate
		{
			OnActiveWorldChanged();
		});
	}

	private void OnActiveWorldChanged()
	{
		StopAllSounds();
	}

	private void CollectParameterUpdaters()
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
				if (baseType == typeof(LoopingSoundParameterUpdater))
				{
					flag = true;
					break;
				}
				baseType = baseType.BaseType;
			}
			if (flag)
			{
				LoopingSoundParameterUpdater loopingSoundParameterUpdater = (LoopingSoundParameterUpdater)Activator.CreateInstance(currentDomainType);
				DebugUtil.Assert(!parameterUpdaters.ContainsKey(loopingSoundParameterUpdater.parameter));
				parameterUpdaters[loopingSoundParameterUpdater.parameter] = loopingSoundParameterUpdater;
			}
		}
	}

	public void UpdateFirstParameter(HandleVector<int>.Handle handle, HashedString parameter, float value)
	{
		Sound data = sounds.GetData(handle);
		data.firstParameterValue = value;
		data.firstParameter = parameter;
		if (data.IsPlaying)
		{
			data.ev.setParameterByID(GetSoundDescription(data.path).GetParameterId(parameter), value);
		}
		sounds.SetData(handle, data);
	}

	public void UpdateSecondParameter(HandleVector<int>.Handle handle, HashedString parameter, float value)
	{
		Sound data = sounds.GetData(handle);
		data.secondParameterValue = value;
		data.secondParameter = parameter;
		if (data.IsPlaying)
		{
			data.ev.setParameterByID(GetSoundDescription(data.path).GetParameterId(parameter), value);
		}
		sounds.SetData(handle, data);
	}

	public void UpdateObjectSelection(HandleVector<int>.Handle handle, Vector3 sound_pos, float vol, bool objectIsSelectedAndVisible)
	{
		Sound data = sounds.GetData(handle);
		data.pos = sound_pos;
		data.vol = vol;
		data.objectIsSelectedAndVisible = objectIsSelectedAndVisible;
		ATTRIBUTES_3D attributes = sound_pos.To3DAttributes();
		if (data.IsPlaying)
		{
			data.ev.set3DAttributes(attributes);
			data.ev.setVolume(vol);
		}
		sounds.SetData(handle, data);
	}

	public void UpdateVelocity(HandleVector<int>.Handle handle, Vector2 velocity)
	{
		Sound data = sounds.GetData(handle);
		data.velocity = velocity;
		sounds.SetData(handle, data);
	}

	public void RenderEveryTick(float dt)
	{
		ListPool<Sound, LoopingSoundManager>.PooledList pooledList = ListPool<Sound, LoopingSoundManager>.Allocate();
		ListPool<int, LoopingSoundManager>.PooledList pooledList2 = ListPool<int, LoopingSoundManager>.Allocate();
		ListPool<int, LoopingSoundManager>.PooledList pooledList3 = ListPool<int, LoopingSoundManager>.Allocate();
		List<Sound> dataList = sounds.GetDataList();
		bool flag = Time.timeScale == 0f;
		SoundCuller soundCuller = CameraController.Instance.soundCuller;
		for (int i = 0; i < dataList.Count; i++)
		{
			Sound sound = dataList[i];
			if (sound.objectIsSelectedAndVisible)
			{
				sound.pos = SoundEvent.AudioHighlightListenerPosition(sound.transform.GetPosition());
				sound.vol = 1f;
			}
			else if (sound.transform != null)
			{
				sound.pos = sound.transform.GetPosition();
				sound.pos.z = 0f;
			}
			if (sound.animController != null)
			{
				Vector3 offset = sound.animController.Offset;
				sound.pos.x += offset.x;
				sound.pos.y += offset.y;
			}
			bool num = !sound.IsCullingEnabled || (sound.ShouldCameraScalePosition && soundCuller.IsAudible(sound.pos, sound.falloffDistanceSq)) || soundCuller.IsAudibleNoCameraScaling(sound.pos, sound.falloffDistanceSq);
			bool isPlaying = sound.IsPlaying;
			if (num)
			{
				pooledList.Add(sound);
				if (!isPlaying)
				{
					sound.ev = KFMOD.CreateInstance(GetSoundDescription(sound.path).path);
					dataList[i] = sound;
					pooledList2.Add(i);
				}
			}
			else if (isPlaying)
			{
				pooledList3.Add(i);
			}
		}
		LoopingSoundParameterUpdater.Sound sound2;
		foreach (int item in pooledList2)
		{
			Sound value = dataList[item];
			SoundDescription soundDescription = GetSoundDescription(value.path);
			value.ev.setPaused(flag && value.ShouldPauseOnGamePaused);
			value.pos.z = 0f;
			Vector3 pos = value.pos;
			if (value.objectIsSelectedAndVisible)
			{
				value.pos = SoundEvent.AudioHighlightListenerPosition(value.transform.GetPosition());
				value.vol = 1f;
			}
			else if (value.transform != null)
			{
				value.pos = value.transform.GetPosition();
			}
			value.ev.set3DAttributes(pos.To3DAttributes());
			value.ev.setVolume(value.vol);
			value.ev.start();
			value.flags |= Sound.Flags.PLAYING;
			if (value.firstParameter != HashedString.Invalid)
			{
				value.ev.setParameterByID(soundDescription.GetParameterId(value.firstParameter), value.firstParameterValue);
			}
			if (value.secondParameter != HashedString.Invalid)
			{
				value.ev.setParameterByID(soundDescription.GetParameterId(value.secondParameter), value.secondParameterValue);
			}
			sound2 = default(LoopingSoundParameterUpdater.Sound);
			sound2.ev = value.ev;
			sound2.path = value.path;
			sound2.description = soundDescription;
			sound2.transform = value.transform;
			sound2.objectIsSelectedAndVisible = false;
			LoopingSoundParameterUpdater.Sound sound3 = sound2;
			SoundDescription.Parameter[] parameters = soundDescription.parameters;
			for (int j = 0; j < parameters.Length; j++)
			{
				SoundDescription.Parameter parameter = parameters[j];
				LoopingSoundParameterUpdater value2 = null;
				if (parameterUpdaters.TryGetValue(parameter.name, out value2))
				{
					value2.Add(sound3);
				}
			}
			dataList[item] = value;
		}
		pooledList2.Recycle();
		foreach (int item2 in pooledList3)
		{
			Sound value3 = dataList[item2];
			SoundDescription soundDescription2 = GetSoundDescription(value3.path);
			sound2 = default(LoopingSoundParameterUpdater.Sound);
			sound2.ev = value3.ev;
			sound2.path = value3.path;
			sound2.description = soundDescription2;
			sound2.transform = value3.transform;
			sound2.objectIsSelectedAndVisible = false;
			LoopingSoundParameterUpdater.Sound sound4 = sound2;
			SoundDescription.Parameter[] parameters = soundDescription2.parameters;
			for (int j = 0; j < parameters.Length; j++)
			{
				SoundDescription.Parameter parameter2 = parameters[j];
				LoopingSoundParameterUpdater value4 = null;
				if (parameterUpdaters.TryGetValue(parameter2.name, out value4))
				{
					value4.Remove(sound4);
				}
			}
			if (value3.ShouldCameraScalePosition)
			{
				value3.ev.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			}
			else
			{
				value3.ev.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			value3.flags &= ~Sound.Flags.PLAYING;
			value3.ev.release();
			dataList[item2] = value3;
		}
		pooledList3.Recycle();
		float velocityScale = TuningData<Tuning>.Get().velocityScale;
		foreach (Sound item3 in pooledList)
		{
			ATTRIBUTES_3D attributes = SoundEvent.GetCameraScaledPosition(item3.pos, item3.objectIsSelectedAndVisible).To3DAttributes();
			attributes.velocity = RuntimeUtils.ToFMODVector(item3.velocity * velocityScale);
			EventInstance ev = item3.ev;
			ev.set3DAttributes(attributes);
		}
		foreach (KeyValuePair<HashedString, LoopingSoundParameterUpdater> parameterUpdater in parameterUpdaters)
		{
			parameterUpdater.Value.Update(dt);
		}
		pooledList.Recycle();
	}

	public static LoopingSoundManager Get()
	{
		return instance;
	}

	public void StopAllSounds()
	{
		foreach (Sound data in sounds.GetDataList())
		{
			if (data.IsPlaying)
			{
				EventInstance ev = data.ev;
				ev.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				ev = data.ev;
				ev.release();
			}
		}
	}

	private SoundDescription GetSoundDescription(HashedString path)
	{
		return KFMOD.GetSoundEventDescription(path);
	}

	public HandleVector<int>.Handle Add(string path, Vector3 pos, Transform transform = null, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true, float vol = 1f, bool objectIsSelectedAndVisible = false)
	{
		SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(path);
		Sound.Flags flags = (Sound.Flags)0;
		if (pause_on_game_pause)
		{
			flags |= Sound.Flags.PAUSE_ON_GAME_PAUSED;
		}
		if (enable_culling)
		{
			flags |= Sound.Flags.ENABLE_CULLING;
		}
		if (enable_camera_scaled_position)
		{
			flags |= Sound.Flags.ENABLE_CAMERA_SCALED_POSITION;
		}
		KBatchedAnimController animController = null;
		if (transform != null)
		{
			animController = transform.GetComponent<KBatchedAnimController>();
		}
		Sound sound = default(Sound);
		sound.transform = transform;
		sound.animController = animController;
		sound.falloffDistanceSq = soundEventDescription.falloffDistanceSq;
		sound.path = path;
		sound.pos = pos;
		sound.flags = flags;
		sound.firstParameter = HashedString.Invalid;
		sound.secondParameter = HashedString.Invalid;
		sound.vol = vol;
		sound.objectIsSelectedAndVisible = objectIsSelectedAndVisible;
		Sound initial_data = sound;
		return sounds.Allocate(initial_data);
	}

	public static HandleVector<int>.Handle StartSound(string path, Vector3 pos, bool pause_on_game_pause = true, bool enable_culling = true)
	{
		if (string.IsNullOrEmpty(path))
		{
			Debug.LogWarning("Missing sound");
			return HandleVector<int>.InvalidHandle;
		}
		return Get().Add(path, pos, null, pause_on_game_pause, enable_culling);
	}

	public static void StopSound(HandleVector<int>.Handle handle)
	{
		if (Get() == null)
		{
			return;
		}
		Sound data = Get().sounds.GetData(handle);
		if (data.IsPlaying)
		{
			data.ev.stop(Get().GameIsPaused ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			data.ev.release();
			SoundDescription soundEventDescription = KFMOD.GetSoundEventDescription(data.path);
			SoundDescription.Parameter[] parameters = soundEventDescription.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				SoundDescription.Parameter parameter = parameters[i];
				LoopingSoundParameterUpdater value = null;
				if (Get().parameterUpdaters.TryGetValue(parameter.name, out value))
				{
					LoopingSoundParameterUpdater.Sound sound = default(LoopingSoundParameterUpdater.Sound);
					sound.ev = data.ev;
					sound.path = data.path;
					sound.description = soundEventDescription;
					sound.transform = data.transform;
					sound.objectIsSelectedAndVisible = false;
					LoopingSoundParameterUpdater.Sound sound2 = sound;
					value.Remove(sound2);
				}
			}
		}
		Get().sounds.Free(handle);
	}

	public static void PauseSound(HandleVector<int>.Handle handle, bool paused)
	{
		Sound data = Get().sounds.GetData(handle);
		if (data.IsPlaying)
		{
			data.ev.setPaused(paused);
		}
	}

	private void OnPauseChanged(object data)
	{
		bool flag = (bool)data;
		GameIsPaused = flag;
		foreach (Sound data2 in sounds.GetDataList())
		{
			if (data2.IsPlaying)
			{
				EventInstance ev = data2.ev;
				ev.setPaused(flag && data2.ShouldPauseOnGamePaused);
			}
		}
	}
}
