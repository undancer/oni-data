using System;
using System.Diagnostics;
using FMOD.Studio;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class SoundEvent : AnimEvent
{
	public static int IGNORE_INTERVAL = -1;

	protected bool isDynamic;

	public string sound
	{
		get;
		private set;
	}

	public HashedString soundHash
	{
		get;
		private set;
	}

	public bool looping
	{
		get;
		private set;
	}

	public bool ignorePause
	{
		get;
		set;
	}

	public bool shouldCameraScalePosition
	{
		get;
		set;
	}

	public float minInterval
	{
		get;
		private set;
	}

	public bool objectIsSelectedAndVisible
	{
		get;
		set;
	}

	public EffectorValues noiseValues
	{
		get;
		set;
	}

	public SoundEvent()
	{
	}

	public SoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, sound_name, frame)
	{
		shouldCameraScalePosition = true;
		if (do_load)
		{
			sound = GlobalAssets.GetSound(sound_name);
			soundHash = new HashedString(sound);
			if (sound != null)
			{
				_ = sound == "";
			}
		}
		minInterval = min_interval;
		looping = is_looping;
		isDynamic = is_dynamic;
		noiseValues = SoundEventVolumeCache.instance.GetVolume(file_name, sound_name);
	}

	public static bool ObjectIsSelectedAndVisible(GameObject go)
	{
		return false;
	}

	public static Vector3 AudioHighlightListenerPosition(Vector3 sound_pos)
	{
		Vector3 position = SoundListenerController.Instance.transform.position;
		float x = 1f * sound_pos.x + 0f * position.x;
		float y = 1f * sound_pos.y + 0f * position.y;
		float z = 0f * position.z;
		return new Vector3(x, y, z);
	}

	public static float GetVolume(bool objectIsSelectedAndVisible)
	{
		float result = 1f;
		if (objectIsSelectedAndVisible)
		{
			result = 1f;
		}
		return result;
	}

	public static bool ShouldPlaySound(KBatchedAnimController controller, string sound, bool is_looping, bool is_dynamic)
	{
		return ShouldPlaySound(controller, sound, sound, is_looping, is_dynamic);
	}

	public static bool ShouldPlaySound(KBatchedAnimController controller, string sound, HashedString soundHash, bool is_looping, bool is_dynamic)
	{
		CameraController instance = CameraController.Instance;
		if (instance == null)
		{
			return true;
		}
		Vector3 position = controller.transform.GetPosition();
		Vector3 offset = controller.Offset;
		position.x += offset.x;
		position.y += offset.y;
		SpeedControlScreen instance2 = SpeedControlScreen.Instance;
		if (is_dynamic)
		{
			if (instance2 != null && instance2.IsPaused)
			{
				return false;
			}
			if (!instance.IsAudibleSound(position))
			{
				return false;
			}
			return true;
		}
		if (sound == null || IsLowPrioritySound(sound))
		{
			return false;
		}
		if (!instance.IsAudibleSound(position, soundHash))
		{
			if (!is_looping && !GlobalAssets.IsHighPriority(sound))
			{
				return false;
			}
		}
		else if (instance2 != null && instance2.IsPaused)
		{
			return false;
		}
		return true;
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		GameObject gameObject = behaviour.controller.gameObject;
		objectIsSelectedAndVisible = ObjectIsSelectedAndVisible(gameObject);
		if (objectIsSelectedAndVisible || ShouldPlaySound(behaviour.controller, sound, soundHash, looping, isDynamic))
		{
			PlaySound(behaviour);
		}
	}

	protected void PlaySound(AnimEventManager.EventPlayerData behaviour, string sound)
	{
		Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
		vector.z = 0f;
		if (ObjectIsSelectedAndVisible(behaviour.controller.gameObject))
		{
			vector = AudioHighlightListenerPosition(vector);
		}
		KBatchedAnimController component = behaviour.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			Vector3 offset = component.Offset;
			vector.x += offset.x;
			vector.y += offset.y;
		}
		AudioDebug audioDebug = AudioDebug.Get();
		if (audioDebug != null && audioDebug.debugSoundEvents)
		{
			Debug.Log(behaviour.name + ", " + sound + ", " + base.frame + ", " + vector);
		}
		try
		{
			if (looping)
			{
				LoopingSounds component2 = behaviour.GetComponent<LoopingSounds>();
				if (component2 == null)
				{
					Debug.Log(behaviour.name + " is missing LoopingSounds component. ");
				}
				else if (!component2.StartSound(sound, behaviour, noiseValues, ignorePause, shouldCameraScalePosition))
				{
					DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{sound}] on behaviour [{behaviour.name}]");
				}
			}
			else if (!PlayOneShot(sound, behaviour, noiseValues, GetVolume(objectIsSelectedAndVisible), objectIsSelectedAndVisible))
			{
				DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{sound}] on behaviour [{behaviour.name}]");
			}
		}
		catch (Exception ex)
		{
			string text = string.Format(("Error trying to trigger sound [{0}] in behaviour [{1}] [{2}]\n{3}" + sound != null) ? sound.ToString() : "null", behaviour.GetType().ToString(), ex.Message, ex.StackTrace);
			Debug.LogError(text);
			throw new ArgumentException(text, ex);
		}
	}

	public virtual void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		PlaySound(behaviour, sound);
	}

	public static Vector3 GetCameraScaledPosition(Vector3 pos, bool objectIsSelectedAndVisible = false)
	{
		Vector3 result = Vector3.zero;
		if (CameraController.Instance != null)
		{
			result = CameraController.Instance.GetVerticallyScaledPosition(pos, objectIsSelectedAndVisible);
		}
		return result;
	}

	public static FMOD.Studio.EventInstance BeginOneShot(string ev, Vector3 pos, float volume = 1f, bool objectIsSelectedAndVisible = false)
	{
		return KFMOD.BeginOneShot(ev, GetCameraScaledPosition(pos, objectIsSelectedAndVisible), volume);
	}

	public static bool EndOneShot(FMOD.Studio.EventInstance instance)
	{
		return KFMOD.EndOneShot(instance);
	}

	public static bool PlayOneShot(string sound, Vector3 sound_pos, float volume = 1f)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(sound))
		{
			FMOD.Studio.EventInstance instance = BeginOneShot(sound, sound_pos, volume);
			if (instance.isValid())
			{
				result = EndOneShot(instance);
			}
		}
		return result;
	}

	public static bool PlayOneShot(string sound, AnimEventManager.EventPlayerData behaviour, EffectorValues noiseValues, float volume = 1f, bool objectIsSelectedAndVisible = false)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(sound))
		{
			Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
			vector.z = 0f;
			if (objectIsSelectedAndVisible)
			{
				vector = AudioHighlightListenerPosition(vector);
			}
			FMOD.Studio.EventInstance instance = BeginOneShot(sound, vector, volume);
			if (instance.isValid())
			{
				result = EndOneShot(instance);
			}
		}
		return result;
	}

	public override void Stop(AnimEventManager.EventPlayerData behaviour)
	{
		if (looping)
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StopSound(sound);
			}
		}
	}

	protected static bool IsLowPrioritySound(string sound)
	{
		if (sound != null && Camera.main.orthographicSize > AudioMixer.LOW_PRIORITY_CUTOFF_DISTANCE && !AudioMixer.instance.activeNIS && GlobalAssets.IsLowPriority(sound))
		{
			return true;
		}
		return false;
	}

	protected void PrintSoundDebug(string anim_name, string sound, string sound_name, Vector3 sound_pos)
	{
		if (sound != null)
		{
			Debug.Log(anim_name + ", " + sound_name + ", " + base.frame + ", " + sound_pos);
		}
		else
		{
			Debug.Log("Missing sound: " + anim_name + ", " + sound_name);
		}
	}
}
