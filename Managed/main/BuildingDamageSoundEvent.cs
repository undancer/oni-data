using System;
using UnityEngine;

[Serializable]
public class BuildingDamageSoundEvent : SoundEvent
{
	public BuildingDamageSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, do_load: false, is_looping: false, SoundEvent.IGNORE_INTERVAL, is_dynamic: false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 sound_pos = behaviour.GetComponent<Transform>().GetPosition();
		sound_pos.z = 0f;
		GameObject gameObject = behaviour.controller.gameObject;
		base.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (base.objectIsSelectedAndVisible)
		{
			sound_pos = SoundEvent.AudioHighlightListenerPosition(sound_pos);
		}
		Worker component = behaviour.GetComponent<Worker>();
		if (component == null)
		{
			string sound = GlobalAssets.GetSound("Building_Dmg_Metal");
			if (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound, base.looping, isDynamic))
			{
				SoundEvent.PlayOneShot(base.sound, sound_pos, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
				return;
			}
		}
		Workable workable = component.workable;
		if (!(workable != null))
		{
			return;
		}
		Building component2 = workable.GetComponent<Building>();
		if (component2 != null)
		{
			BuildingDef def = component2.Def;
			string sound2 = GlobalAssets.GetSound(StringFormatter.Combine(base.name, "_", def.AudioCategory));
			if (sound2 == null)
			{
				sound2 = GlobalAssets.GetSound("Building_Dmg_Metal");
			}
			if (sound2 != null && (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound2, base.looping, isDynamic)))
			{
				SoundEvent.PlayOneShot(sound2, sound_pos, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
			}
		}
	}
}
