using TUNING;
using UnityEngine;

public class FacilityBackWallWindowConfig : IBuildingConfig
{
	public const string ID = "FacilityBackWallWindow";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("FacilityBackWallWindow", 1, 6, "gravitas_window_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.GLASSES, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: DECOR.BONUS.TIER3);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.BaseTimeUntilRepair = -1f;
		obj.DefaultAnimState = "off";
		obj.ObjectLayer = ObjectLayer.Backwall;
		obj.SceneLayer = Grid.SceneLayer.Backwall;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
		go.AddComponent<ZoneTile>();
		go.GetComponent<PrimaryElement>().SetElement(SimHashes.Steel);
		go.GetComponent<PrimaryElement>().Temperature = 273f;
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
