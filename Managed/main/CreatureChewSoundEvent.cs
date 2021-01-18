using FMOD.Studio;
using UnityEngine;

public class CreatureChewSoundEvent : SoundEvent
{
	private static string DEFAULT_CHEW_SOUND = "Rock";

	private const string FMOD_PARAM_IS_BABY_ID = "isBaby";

	public CreatureChewSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, do_load: false, is_looping: false, min_interval, is_dynamic: true)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		string sound = GlobalAssets.GetSound(StringFormatter.Combine(base.name, "_", GetChewSound(behaviour)));
		GameObject gameObject = behaviour.controller.gameObject;
		base.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound, base.looping, isDynamic))
		{
			Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
			vector.z = 0f;
			if (base.objectIsSelectedAndVisible)
			{
				vector = SoundEvent.AudioHighlightListenerPosition(vector);
			}
			EventInstance instance = SoundEvent.BeginOneShot(sound, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
			if (behaviour.controller.gameObject.GetDef<BabyMonitor.Def>() != null)
			{
				instance.setParameterByName("isBaby", 1f);
			}
			SoundEvent.EndOneShot(instance);
		}
	}

	private static string GetChewSound(AnimEventManager.EventPlayerData behaviour)
	{
		string result = DEFAULT_CHEW_SOUND;
		EatStates.Instance sMI = behaviour.controller.GetSMI<EatStates.Instance>();
		if (sMI != null)
		{
			Element latestMealElement = sMI.GetLatestMealElement();
			if (latestMealElement != null)
			{
				string creatureChewSound = latestMealElement.substance.GetCreatureChewSound();
				if (!string.IsNullOrEmpty(creatureChewSound))
				{
					result = creatureChewSound;
				}
			}
		}
		return result;
	}
}
