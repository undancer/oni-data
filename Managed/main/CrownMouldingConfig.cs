using TUNING;
using UnityEngine;

public class CrownMouldingConfig : IBuildingConfig
{
	public const string ID = "CrownMoulding";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("CrownMoulding", 1, 1, "crown_moulding_kanim", 10, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnCeiling, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = 5,
			radius = 3
		});
		obj.DefaultAnimState = "S_U";
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.Decor.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		return obj;
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
