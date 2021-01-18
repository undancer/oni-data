using System.Diagnostics;
using FMOD.Studio;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class FloorSoundEvent : SoundEvent
{
	public static float IDLE_WALKING_VOLUME_REDUCTION = 0.55f;

	public FloorSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, do_load: false, is_looping: false, SoundEvent.IGNORE_INTERVAL, is_dynamic: true)
	{
		base.noiseValues = SoundEventVolumeCache.instance.GetVolume("FloorSoundEvent", sound_name);
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
		KBatchedAnimController component = behaviour.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			vector = component.GetPivotSymbolPosition();
		}
		int num = Grid.PosToCell(vector);
		int cell = Grid.CellBelow(num);
		string audioCategory = GetAudioCategory(cell);
		string name = StringFormatter.Combine(audioCategory, "_", base.name);
		string sound = GlobalAssets.GetSound(name, force_no_warning: true);
		if (sound == null)
		{
			name = StringFormatter.Combine("Rock_", base.name);
			sound = GlobalAssets.GetSound(name, force_no_warning: true);
			if (sound == null)
			{
				name = base.name;
				sound = GlobalAssets.GetSound(name, force_no_warning: true);
			}
		}
		GameObject gameObject = behaviour.controller.gameObject;
		base.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (SoundEvent.IsLowPrioritySound(sound) && !base.objectIsSelectedAndVisible)
		{
			return;
		}
		vector = SoundEvent.GetCameraScaledPosition(vector);
		vector.z = 0f;
		if (base.objectIsSelectedAndVisible)
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
		}
		if (Grid.Element == null)
		{
			return;
		}
		bool isLiquid = Grid.Element[num].IsLiquid;
		float num2 = 0f;
		if (isLiquid)
		{
			num2 = SoundUtil.GetLiquidDepth(num);
			string sound2 = GlobalAssets.GetSound("Liquid_footstep", force_no_warning: true);
			if (sound2 != null && (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound2, base.looping, isDynamic)))
			{
				FMOD.Studio.EventInstance instance = SoundEvent.BeginOneShot(sound2, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
				if (num2 > 0f)
				{
					instance.setParameterByName("liquidDepth", num2);
				}
				SoundEvent.EndOneShot(instance);
			}
		}
		if (sound == null || (!base.objectIsSelectedAndVisible && !SoundEvent.ShouldPlaySound(behaviour.controller, sound, base.looping, isDynamic)))
		{
			return;
		}
		FMOD.Studio.EventInstance instance2 = SoundEvent.BeginOneShot(sound, vector);
		if (instance2.isValid())
		{
			if (num2 > 0f)
			{
				instance2.setParameterByName("liquidDepth", num2);
			}
			if (behaviour.currentAnimFile != null && behaviour.currentAnimFile.Contains("anim_loco_walk"))
			{
				instance2.setVolume(IDLE_WALKING_VOLUME_REDUCTION);
			}
			SoundEvent.EndOneShot(instance2);
		}
	}

	private static string GetAudioCategory(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return "Rock";
		}
		Element element = Grid.Element[cell];
		if (Grid.Foundation[cell])
		{
			BuildingDef buildingDef = null;
			GameObject gameObject = Grid.Objects[cell, 1];
			if (gameObject != null)
			{
				Building component = gameObject.GetComponent<BuildingComplete>();
				if (component != null)
				{
					buildingDef = component.Def;
				}
			}
			string result = "";
			if (buildingDef != null)
			{
				result = buildingDef.PrefabID switch
				{
					"PlasticTile" => "TilePlastic", 
					"GlassTile" => "TileGlass", 
					"BunkerTile" => "TileBunker", 
					"MetalTile" => "TileMetal", 
					"CarpetTile" => "Carpet", 
					_ => "Tile", 
				};
			}
			return result;
		}
		string floorEventAudioCategory = element.substance.GetFloorEventAudioCategory();
		if (floorEventAudioCategory != null)
		{
			return floorEventAudioCategory;
		}
		if (element.HasTag(GameTags.RefinedMetal))
		{
			return "RefinedMetal";
		}
		if (element.HasTag(GameTags.Metal))
		{
			return "RawMetal";
		}
		return "Rock";
	}
}
