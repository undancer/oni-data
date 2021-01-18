using TUNING;
using UnityEngine;

public class POIDoorInternalConfig : IBuildingConfig
{
	public static string ID = "POIDoorInternal";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "door_poi_internal_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE, temperature_modification_mass_scale: 1f);
		obj.ShowInBuildMenu = false;
		obj.Entombable = false;
		obj.Floodable = false;
		obj.Invincible = true;
		obj.IsFoundation = true;
		obj.AudioCategory = "Metal";
		obj.PermittedRotations = PermittedRotations.R90;
		obj.SceneLayer = Grid.SceneLayer.TileMain;
		obj.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		obj.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(0, 0));
		SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.Internal;
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<AccessControl>();
		go.AddOrGet<Workable>().workTime = 3f;
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		AccessControl component = go.GetComponent<AccessControl>();
		go.GetComponent<Door>().hasComplexUserControls = false;
		component.controlEnabled = false;
		go.GetComponent<Deconstructable>().allowDeconstruction = true;
		go.GetComponent<KBatchedAnimController>().initialAnim = "closed";
	}
}
