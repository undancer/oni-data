using TUNING;
using UnityEngine;

public class CrownMouldingConfig : IBuildingConfig
{
	public const string ID = "CrownMoulding";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CrownMoulding", 1, 1, "crown_moulding_kanim", 10, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnCeiling, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = 5,
			radius = 3
		});
		buildingDef.DefaultAnimState = "S_U";
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
