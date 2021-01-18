using TUNING;
using UnityEngine;

public class BunkerDoorConfig : IBuildingConfig
{
	public const string ID = "BunkerDoor";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("BunkerDoor", 4, 1, "door_bunker_kanim", 1000, 120f, new float[1]
		{
			500f
		}, new string[1]
		{
			SimHashes.Steel.ToString()
		}, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE, temperature_modification_mass_scale: 1f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.OverheatTemperature = 1273.15f;
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(-1, 0));
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
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
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 3f;
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.initialAnim = "closed";
		component.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		KPrefabID component2 = go.GetComponent<KPrefabID>();
		component2.AddTag(GameTags.Bunker);
	}
}
