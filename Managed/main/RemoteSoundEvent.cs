using System;
using FMOD.Studio;
using UnityEngine;

[Serializable]
public class RemoteSoundEvent : SoundEvent
{
	private const string STATE_PARAMETER = "State";

	public RemoteSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, do_load: true, is_looping: false, min_interval, is_dynamic: false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
		vector.z = 0f;
		GameObject gameObject = behaviour.controller.gameObject;
		if (SoundEvent.ObjectIsSelectedAndVisible(gameObject))
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
		}
		Workable workable = behaviour.GetComponent<Worker>().workable;
		if (!(workable != null))
		{
			return;
		}
		Toggleable component = workable.GetComponent<Toggleable>();
		if (component != null)
		{
			IToggleHandler toggleHandlerForWorker = component.GetToggleHandlerForWorker(behaviour.GetComponent<Worker>());
			float value = 1f;
			if (toggleHandlerForWorker?.IsHandlerOn() ?? false)
			{
				value = 0f;
			}
			if (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, base.sound, base.soundHash, base.looping, isDynamic))
			{
				EventInstance instance = SoundEvent.BeginOneShot(base.sound, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
				instance.setParameterByName("State", value);
				SoundEvent.EndOneShot(instance);
			}
		}
	}
}
