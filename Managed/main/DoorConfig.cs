using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DoorConfig : IBuildingConfig
{
	public const string ID = "Door";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("Door", 1, 2, "door_internal_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.NONE, temperature_modification_mass_scale: 1f);
		obj.Entombable = true;
		obj.Floodable = false;
		obj.IsFoundation = false;
		obj.AudioCategory = "Metal";
		obj.PermittedRotations = PermittedRotations.R90;
		obj.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		obj.LogicInputPorts = CreateSingleInputPortList(new CellOffset(0, 0));
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		return obj;
	}

	public static List<LogicPorts.Port> CreateSingleInputPortList(CellOffset offset)
	{
		return new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(Door.OPEN_CLOSE_PORT_ID, offset, STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN, STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN_ACTIVE, STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN_INACTIVE)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.Internal;
		door.doorOpeningSoundEventName = "Open_DoorInternal";
		door.doorClosingSoundEventName = "Close_DoorInternal";
		go.AddOrGet<AccessControl>().controlEnabled = true;
		go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Door;
		go.AddOrGet<Workable>().workTime = 3f;
		go.GetComponent<KBatchedAnimController>().initialAnim = "closed";
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}
}
