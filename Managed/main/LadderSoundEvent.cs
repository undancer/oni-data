using UnityEngine;

public class LadderSoundEvent : SoundEvent
{
	public LadderSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, do_load: false, is_looping: false, SoundEvent.IGNORE_INTERVAL, is_dynamic: true)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		GameObject gameObject = behaviour.controller.gameObject;
		base.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (!base.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, base.sound, base.looping, isDynamic))
		{
			return;
		}
		Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
		vector.z = 0f;
		float volume = 1f;
		if (base.objectIsSelectedAndVisible)
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
			volume = SoundEvent.GetVolume(base.objectIsSelectedAndVisible);
		}
		int cell = Grid.PosToCell(vector);
		GameObject gameObject2 = null;
		BuildingDef buildingDef = null;
		if (Grid.IsValidCell(cell))
		{
			gameObject2 = Grid.Objects[cell, 1];
			if (gameObject2 != null && gameObject2.GetComponent<Ladder>() != null)
			{
				Building component = gameObject2.GetComponent<BuildingComplete>();
				if (component != null)
				{
					buildingDef = component.Def;
				}
			}
		}
		if (buildingDef != null)
		{
			string sound = GlobalAssets.GetSound((buildingDef.PrefabID == "LadderFast") ? StringFormatter.Combine(base.name, "_Plastic") : base.name);
			if (sound != null)
			{
				SoundEvent.PlayOneShot(sound, vector, volume);
			}
		}
	}
}
