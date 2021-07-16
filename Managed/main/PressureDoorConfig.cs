using TUNING;
using UnityEngine;

public class PressureDoorConfig : IBuildingConfig
{
	public const string ID = "PressureDoor";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("PressureDoor", 1, 2, "door_external_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1, temperature_modification_mass_scale: 1f);
		obj.Overheatable = false;
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.IsFoundation = true;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.TileLayer = ObjectLayer.FoundationTile;
		obj.AudioCategory = "Metal";
		obj.PermittedRotations = PermittedRotations.R90;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		obj.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(0, 0));
		SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Open_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Close_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Door door = go.AddOrGet<Door>();
		door.hasComplexUserControls = true;
		door.unpoweredAnimSpeed = 0.65f;
		door.poweredAnimSpeed = 5f;
		door.doorClosingSoundEventName = "MechanizedAirlock_closing";
		door.doorOpeningSoundEventName = "MechanizedAirlock_opening";
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<AccessControl>();
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Door;
		go.AddOrGet<Workable>().workTime = 5f;
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		go.GetComponent<AccessControl>().controlEnabled = true;
		go.GetComponent<KBatchedAnimController>().initialAnim = "closed";
	}
}
