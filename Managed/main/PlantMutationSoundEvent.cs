using UnityEngine;

public class PlantMutationSoundEvent : SoundEvent
{
	public PlantMutationSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, do_load: false, is_looping: false, min_interval, is_dynamic: true)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		GameObject gameObject = behaviour.controller.gameObject;
		MutantPlant component = gameObject.GetComponent<MutantPlant>();
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		if (component != null)
		{
			for (int i = 0; i < component.GetSoundEvents().Count; i++)
			{
				SoundEvent.PlayOneShot(component.GetSoundEvents()[i], position);
			}
		}
	}
}
