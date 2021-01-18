using FMOD.Studio;
using UnityEngine;

public class HatchDrillSoundEvent : SoundEvent
{
	public HatchDrillSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, do_load: true, is_looping: true, min_interval, is_dynamic: false)
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
		int cell = Grid.PosToCell(vector);
		int cell2 = Grid.CellBelow(cell);
		float value = GetAudioCategory(cell2);
		EventInstance instance = SoundEvent.BeginOneShot(base.sound, vector);
		instance.setParameterByName("material_ID", value);
		SoundEvent.EndOneShot(instance);
	}

	private static int GetAudioCategory(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return 7;
		}
		Element element = Grid.Element[cell];
		if (element.id == SimHashes.Dirt)
		{
			return 0;
		}
		if (element.HasTag(GameTags.IceOre))
		{
			return 1;
		}
		if (element.id == SimHashes.CrushedIce)
		{
			return 12;
		}
		if (element.id == SimHashes.DirtyIce)
		{
			return 13;
		}
		if (Grid.Foundation[cell])
		{
			return 2;
		}
		if (element.id == SimHashes.OxyRock)
		{
			return 3;
		}
		if (element.id == SimHashes.PhosphateNodules || element.id == SimHashes.Phosphorus || element.id == SimHashes.Phosphorite)
		{
			return 4;
		}
		if (element.HasTag(GameTags.Metal))
		{
			return 5;
		}
		if (element.HasTag(GameTags.RefinedMetal))
		{
			return 6;
		}
		if (element.id == SimHashes.Sand)
		{
			return 8;
		}
		if (element.id == SimHashes.Clay)
		{
			return 9;
		}
		if (element.id == SimHashes.Algae)
		{
			return 10;
		}
		if (element.id == SimHashes.SlimeMold)
		{
			return 11;
		}
		return 7;
	}
}
