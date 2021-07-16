using TUNING;
using UnityEngine;

public class CornerMouldingConfig : IBuildingConfig
{
	public const string ID = "CornerMoulding";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("CornerMoulding", 1, 1, "corner_tile_kanim", 10, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.InCorner, noise: NOISE_POLLUTION.NONE, decor: new EffectorValues
		{
			amount = 5,
			radius = 3
		});
		obj.DefaultAnimState = "corner";
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.Decor.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.PermittedRotations = PermittedRotations.FlipH;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
