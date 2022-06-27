using TUNING;
using UnityEngine;

public class PropGravitasWallConfig : IBuildingConfig
{
	public const string ID = "PropGravitasWall";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("PropGravitasWall", 1, 1, "gravitas_walls_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: DECOR.BONUS.TIER0);
		obj.PermittedRotations = PermittedRotations.R360;
		obj.Entombable = false;
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.BaseTimeUntilRepair = -1f;
		obj.DefaultAnimState = "off";
		obj.ObjectLayer = ObjectLayer.Backwall;
		obj.SceneLayer = Grid.SceneLayer.Backwall;
		obj.ShowInBuildMenu = false;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
		go.AddComponent<ZoneTile>();
		go.GetComponent<PrimaryElement>().SetElement(SimHashes.Granite);
		go.GetComponent<PrimaryElement>().Temperature = 273f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Gravitas);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
