using TUNING;
using UnityEngine;

public class BottleEmptierConfig : IBuildingConfig
{
	public const string ID = "BottleEmptier";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("BottleEmptier", 1, 3, "liquidator_kanim", 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.Floodable = false;
		obj.AudioCategory = "Metal";
		obj.Overheatable = false;
		obj.PermittedRotations = PermittedRotations.FlipH;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.storageFilters = STORAGEFILTERS.LIQUIDS;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.capacityKg = 200f;
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<BottleEmptier>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
