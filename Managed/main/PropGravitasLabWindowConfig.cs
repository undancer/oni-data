using TUNING;
using UnityEngine;

public class PropGravitasLabWindowConfig : IBuildingConfig
{
	public const string ID = "PropGravitasLabWindow";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("PropGravitasLabWindow", 2, 3, "gravitas_lab_window_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY, MATERIALS.GLASSES, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: DECOR.BONUS.TIER0);
		obj.Entombable = false;
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.BaseTimeUntilRepair = -1f;
		obj.DefaultAnimState = "on";
		obj.ObjectLayer = ObjectLayer.Backwall;
		obj.SceneLayer = Grid.SceneLayer.Backwall;
		obj.ShowInBuildMenu = false;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
		go.AddComponent<ZoneTile>();
		go.GetComponent<PrimaryElement>().SetElement(SimHashes.Glass);
		go.GetComponent<PrimaryElement>().Temperature = 273f;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Gravitas);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
