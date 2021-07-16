using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PhonoboxSoundEvent : SoundEvent
{
	private const string SOUND_PARAM_SONG = "jukeboxSong";

	private const string SOUND_PARAM_PITCH = "jukeboxPitch";

	private int song;

	private int pitch;

	public PhonoboxSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, do_load: true, is_looping: true, min_interval, is_dynamic: false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		position.z = 0f;
		AudioDebug audioDebug = AudioDebug.Get();
		if (audioDebug != null && audioDebug.debugSoundEvents)
		{
			string[] obj = new string[7]
			{
				behaviour.name,
				", ",
				base.sound,
				", ",
				base.frame.ToString(),
				", ",
				null
			};
			Vector3 vector = position;
			obj[6] = vector.ToString();
			Debug.Log(string.Concat(obj));
		}
		try
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component == null)
			{
				Debug.Log(behaviour.name + " is missing LoopingSounds component. ");
			}
			else if (!component.IsSoundPlaying(base.sound))
			{
				if (component.StartSound(base.sound, behaviour, base.noiseValues, base.ignorePause))
				{
					EventDescription eventDescription = RuntimeManager.GetEventDescription(base.sound);
					eventDescription.getParameterDescriptionByName("jukeboxSong", out var parameter);
					int num = (int)parameter.maximum;
					eventDescription.getParameterDescriptionByName("jukeboxPitch", out var parameter2);
					int num2 = (int)parameter2.maximum;
					song = UnityEngine.Random.Range(0, num + 1);
					pitch = UnityEngine.Random.Range(0, num2 + 1);
					component.UpdateFirstParameter(base.sound, "jukeboxSong", song);
					component.UpdateSecondParameter(base.sound, "jukeboxPitch", pitch);
				}
				else
				{
					DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{base.sound}] on behaviour [{behaviour.name}]");
				}
			}
		}
		catch (Exception ex)
		{
			string text = string.Format(("Error trying to trigger sound [{0}] in behaviour [{1}] [{2}]\n{3}" + base.sound != null) ? base.sound.ToString() : "null", behaviour.GetType().ToString(), ex.Message, ex.StackTrace);
			Debug.LogError(text);
			throw new ArgumentException(text, ex);
		}
	}
}
