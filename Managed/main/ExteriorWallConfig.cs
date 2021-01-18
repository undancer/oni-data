using TUNING;
using UnityEngine;

public class ExteriorWallConfig : IBuildingConfig
{
	public const string ID = "ExteriorWall";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ExteriorWall", 1, 1, "walls_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: DECOR.NONE);
		obj.Entombable = false;
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.DefaultAnimState = "off";
		obj.ObjectLayer = ObjectLayer.Backwall;
		obj.SceneLayer = Grid.SceneLayer.Backwall;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
		go.AddComponent<ZoneTile>();
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RemoveLoopingSounds(go);
	}
}
