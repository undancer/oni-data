using FMOD.Studio;
using UnityEngine;

public class UIAnimationSoundEvent : SoundEvent
{
	private static string X_POSITION_PARAMETER = "Screen_Position_X";

	private static string Y_POSITION_PARAMETER = "Screen_Position_Y";

	public UIAnimationSoundEvent(string file_name, string sound_name, int frame, bool looping)
		: base(file_name, sound_name, frame, do_load: true, looping, SoundEvent.IGNORE_INTERVAL, is_dynamic: false)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		PlaySound(behaviour);
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		if (base.looping)
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component == null)
			{
				Debug.Log(behaviour.name + " (UI Object) is missing LoopingSounds component.");
			}
			else if (!component.StartSound(base.sound, pause_on_game_pause: false, enable_culling: false, enable_camera_scaled_position: false))
			{
				DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{base.sound}] on behaviour [{behaviour.name}]");
			}
		}
		else
		{
			EventInstance instance = KFMOD.BeginOneShot(base.sound, Vector3.zero);
			instance.setParameterByName(X_POSITION_PARAMETER, behaviour.controller.transform.GetPosition().x / (float)Screen.width);
			instance.setParameterByName(Y_POSITION_PARAMETER, behaviour.controller.transform.GetPosition().y / (float)Screen.height);
			KFMOD.EndOneShot(instance);
		}
	}

	public override void Stop(AnimEventManager.EventPlayerData behaviour)
	{
		if (base.looping)
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StopSound(base.sound);
			}
		}
	}
}
