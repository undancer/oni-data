using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/LoopingSounds")]
public class LoopingSounds : KMonoBehaviour
{
	private struct LoopingSoundEvent
	{
		public string asset;

		public HandleVector<int>.Handle handle;
	}

	private List<LoopingSoundEvent> loopingSounds = new List<LoopingSoundEvent>();

	private Dictionary<HashedString, float> lastTimePlayed = new Dictionary<HashedString, float>();

	[SerializeField]
	public bool updatePosition;

	public float vol = 1f;

	public bool objectIsSelectedAndVisible;

	public Vector3 sound_pos;

	public bool IsSoundPlaying(string path)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			if (loopingSound.asset == path)
			{
				return true;
			}
		}
		return false;
	}

	public bool StartSound(string asset, AnimEventManager.EventPlayerData behaviour, EffectorValues noiseValues, bool ignore_pause = false, bool enable_camera_scaled_position = true)
	{
		if (asset == null || asset == "")
		{
			Debug.LogWarning("Missing sound");
			return false;
		}
		if (!IsSoundPlaying(asset))
		{
			LoopingSoundEvent loopingSoundEvent = default(LoopingSoundEvent);
			loopingSoundEvent.asset = asset;
			LoopingSoundEvent item = loopingSoundEvent;
			GameObject gameObject = base.gameObject;
			objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
			if (objectIsSelectedAndVisible)
			{
				sound_pos = SoundEvent.AudioHighlightListenerPosition(base.transform.GetPosition());
				vol = SoundEvent.GetVolume(objectIsSelectedAndVisible);
			}
			else
			{
				sound_pos = behaviour.GetComponent<Transform>().GetPosition();
				sound_pos.z = 0f;
			}
			item.handle = LoopingSoundManager.Get().Add(asset, sound_pos, base.transform, !ignore_pause, enable_culling: true, enable_camera_scaled_position, vol, objectIsSelectedAndVisible);
			loopingSounds.Add(item);
		}
		return true;
	}

	public bool StartSound(string asset)
	{
		if (asset == null || asset == "")
		{
			Debug.LogWarning("Missing sound");
			return false;
		}
		if (!IsSoundPlaying(asset))
		{
			LoopingSoundEvent loopingSoundEvent = default(LoopingSoundEvent);
			loopingSoundEvent.asset = asset;
			LoopingSoundEvent item = loopingSoundEvent;
			GameObject gameObject = base.gameObject;
			objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
			if (objectIsSelectedAndVisible)
			{
				sound_pos = SoundEvent.AudioHighlightListenerPosition(base.transform.GetPosition());
				vol = SoundEvent.GetVolume(objectIsSelectedAndVisible);
			}
			else
			{
				sound_pos = base.transform.GetPosition();
				sound_pos.z = 0f;
			}
			item.handle = LoopingSoundManager.Get().Add(asset, sound_pos, base.transform, pause_on_game_pause: true, enable_culling: true, enable_camera_scaled_position: true, vol, objectIsSelectedAndVisible);
			loopingSounds.Add(item);
		}
		return true;
	}

	public bool StartSound(string asset, bool pause_on_game_pause = true, bool enable_culling = true, bool enable_camera_scaled_position = true)
	{
		if (asset == null || asset == "")
		{
			Debug.LogWarning("Missing sound");
			return false;
		}
		if (!IsSoundPlaying(asset))
		{
			LoopingSoundEvent loopingSoundEvent = default(LoopingSoundEvent);
			loopingSoundEvent.asset = asset;
			LoopingSoundEvent item = loopingSoundEvent;
			GameObject gameObject = base.gameObject;
			objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
			if (objectIsSelectedAndVisible)
			{
				sound_pos = SoundEvent.AudioHighlightListenerPosition(base.transform.GetPosition());
				vol = SoundEvent.GetVolume(objectIsSelectedAndVisible);
			}
			else
			{
				sound_pos = base.transform.GetPosition();
				sound_pos.z = 0f;
			}
			item.handle = LoopingSoundManager.Get().Add(asset, sound_pos, base.transform, pause_on_game_pause, enable_culling, enable_camera_scaled_position, vol, objectIsSelectedAndVisible);
			loopingSounds.Add(item);
		}
		return true;
	}

	public void UpdateVelocity(string asset, Vector2 value)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			if (loopingSound.asset == asset)
			{
				LoopingSoundManager.Get().UpdateVelocity(loopingSound.handle, value);
				break;
			}
		}
	}

	public void UpdateFirstParameter(string asset, HashedString parameter, float value)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			if (loopingSound.asset == asset)
			{
				LoopingSoundManager.Get().UpdateFirstParameter(loopingSound.handle, parameter, value);
				break;
			}
		}
	}

	public void UpdateSecondParameter(string asset, HashedString parameter, float value)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			if (loopingSound.asset == asset)
			{
				LoopingSoundManager.Get().UpdateSecondParameter(loopingSound.handle, parameter, value);
				break;
			}
		}
	}

	private void StopSoundAtIndex(int i)
	{
		LoopingSoundManager.StopSound(loopingSounds[i].handle);
	}

	public void StopSound(string asset)
	{
		for (int i = 0; i < loopingSounds.Count; i++)
		{
			if (loopingSounds[i].asset == asset)
			{
				StopSoundAtIndex(i);
				loopingSounds.RemoveAt(i);
				break;
			}
		}
	}

	public void PauseSound(string asset, bool paused)
	{
		for (int i = 0; i < loopingSounds.Count; i++)
		{
			if (loopingSounds[i].asset == asset)
			{
				LoopingSoundManager.PauseSound(loopingSounds[i].handle, paused);
				break;
			}
		}
	}

	public void StopAllSounds()
	{
		for (int i = 0; i < loopingSounds.Count; i++)
		{
			StopSoundAtIndex(i);
		}
		loopingSounds.Clear();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		StopAllSounds();
	}

	public void SetParameter(string path, HashedString parameter, float value)
	{
		foreach (LoopingSoundEvent loopingSound in loopingSounds)
		{
			if (loopingSound.asset == path)
			{
				LoopingSoundManager.Get().UpdateFirstParameter(loopingSound.handle, parameter, value);
				break;
			}
		}
	}

	public void PlayEvent(GameSoundEvents.Event ev)
	{
		if (AudioDebug.Get().debugGameEventSounds)
		{
			HashedString name = ev.Name;
			Debug.Log("GameSoundEvent: " + name.ToString());
		}
		List<AnimEvent> events = GameAudioSheets.Get().GetEvents(ev.Name);
		if (events == null)
		{
			return;
		}
		Vector2 v = base.transform.GetPosition();
		for (int i = 0; i < events.Count; i++)
		{
			SoundEvent soundEvent = events[i] as SoundEvent;
			if (soundEvent == null || soundEvent.sound == null)
			{
				break;
			}
			if (!CameraController.Instance.IsAudibleSound(v, soundEvent.sound))
			{
				continue;
			}
			if (AudioDebug.Get().debugGameEventSounds)
			{
				Debug.Log("GameSound: " + soundEvent.sound);
			}
			float value = 0f;
			if (lastTimePlayed.TryGetValue(soundEvent.soundHash, out value))
			{
				if (Time.time - value > soundEvent.minInterval)
				{
					SoundEvent.PlayOneShot(soundEvent.sound, v);
				}
			}
			else
			{
				SoundEvent.PlayOneShot(soundEvent.sound, v);
			}
			lastTimePlayed[soundEvent.soundHash] = Time.time;
		}
	}

	public void UpdateObjectSelection(bool selected)
	{
		GameObject gameObject = base.gameObject;
		if (selected && gameObject != null && CameraController.Instance.IsVisiblePos(gameObject.transform.position))
		{
			objectIsSelectedAndVisible = true;
			sound_pos = SoundEvent.AudioHighlightListenerPosition(sound_pos);
			vol = 1f;
		}
		else
		{
			objectIsSelectedAndVisible = false;
			sound_pos = base.transform.GetPosition();
			sound_pos.z = 0f;
			vol = 1f;
		}
		for (int i = 0; i < loopingSounds.Count; i++)
		{
			LoopingSoundManager.Get().UpdateObjectSelection(loopingSounds[i].handle, sound_pos, vol, objectIsSelectedAndVisible);
		}
	}
}
