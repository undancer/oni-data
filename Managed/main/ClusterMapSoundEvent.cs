using FMOD.Studio;
using UnityEngine;

public class ClusterMapSoundEvent : SoundEvent
{
	private static string X_POSITION_PARAMETER = "Starmap_Position_X";

	private static string Y_POSITION_PARAMETER = "Starmap_Position_Y";

	private static string ZOOM_PARAMETER = "Starmap_Zoom_Percentage";

	public ClusterMapSoundEvent(string file_name, string sound_name, int frame, bool looping)
		: base(file_name, sound_name, frame, do_load: true, looping, SoundEvent.IGNORE_INTERVAL, is_dynamic: false)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (ClusterMapScreen.Instance.IsActive())
		{
			PlaySound(behaviour);
		}
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		if (base.looping)
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component == null)
			{
				Debug.Log(behaviour.name + " (Cluster Map Object) is missing LoopingSounds component.");
			}
			else if (!component.StartSound(base.sound, pause_on_game_pause: true, enable_culling: false, enable_camera_scaled_position: false))
			{
				DebugUtil.LogWarningArgs($"SoundEvent has invalid sound [{base.sound}] on behaviour [{behaviour.name}]");
			}
		}
		else
		{
			EventInstance instance = KFMOD.BeginOneShot(base.sound, Vector3.zero);
			instance.setParameterByName(X_POSITION_PARAMETER, behaviour.controller.transform.GetPosition().x / (float)Screen.width);
			instance.setParameterByName(Y_POSITION_PARAMETER, behaviour.controller.transform.GetPosition().y / (float)Screen.height);
			instance.setParameterByName(ZOOM_PARAMETER, ClusterMapScreen.Instance.CurrentZoomPercentage());
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
