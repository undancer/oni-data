using FMOD.Studio;
using UnityEngine;

public class WallDamageSoundEvent : SoundEvent
{
	public int tile;

	public WallDamageSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, do_load: true, is_looping: false, min_interval, is_dynamic: false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 vector = default(Vector3);
		int num = 0;
		AggressiveChore.StatesInstance sMI = behaviour.controller.gameObject.GetSMI<AggressiveChore.StatesInstance>();
		if (sMI != null)
		{
			tile = sMI.sm.wallCellToBreak;
			num = GetAudioCategory(tile);
			vector = Grid.CellToPos(tile);
			vector.z = 0f;
			_ = behaviour.controller.gameObject;
			if (base.objectIsSelectedAndVisible)
			{
				vector = SoundEvent.AudioHighlightListenerPosition(vector);
			}
			if (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, base.sound, base.soundHash, base.looping, isDynamic))
			{
				EventInstance instance = SoundEvent.BeginOneShot(base.sound, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
				instance.setParameterByName("material_ID", num);
				SoundEvent.EndOneShot(instance);
			}
		}
	}

	private static int GetAudioCategory(int tile)
	{
		Element element = Grid.Element[tile];
		if (Grid.Foundation[tile])
		{
			return 12;
		}
		if (element.id == SimHashes.Dirt)
		{
			return 0;
		}
		if (element.id == SimHashes.CrushedIce || element.id == SimHashes.Ice || element.id == SimHashes.DirtyIce)
		{
			return 1;
		}
		if (element.id == SimHashes.OxyRock)
		{
			return 3;
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
		if (element.id == SimHashes.Algae)
		{
			return 10;
		}
		return 7;
	}
}
