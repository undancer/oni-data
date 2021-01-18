using TUNING;
using UnityEngine;

public class BunkerDoorConfig : IBuildingConfig
{
	public const string ID = "BunkerDoor";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("BunkerDoor", 4, 1, "door_bunker_kanim", 1000, 120f, new float[1]
		{
			500f
		}, new string[1]
		{
			SimHashes.Steel.ToString()
		}, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE, temperature_modification_mass_scale: 1f);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.OverheatTemperature = 1273.15f;
		obj.Entombable = false;
		obj.IsFoundation = true;
		obj.AudioCategory = "Metal";
		obj.PermittedRotations = PermittedRotations.R90;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		obj.TileLayer = ObjectLayer.FoundationTile;
		obj.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(-1, 0));
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 0.01f;
		door.poweredAnimSpeed = 0.1f;
		door.hasComplexUserControls = true;
		door.allowAutoControl = false;
		door.doorOpeningSoundEventName = "BunkerDoor_opening";
		door.doorClosingSoundEventName = "BunkerDoor_closing";
		door.verticalOrientation = Orientation.R90;
		go.AddOrGet<Workable>().workTime = 3f;
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.initialAnim = "closed";
		component.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		go.GetComponent<KPrefabID>().AddTag(GameTags.Bunker);
	}
}
