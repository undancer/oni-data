using TUNING;
using UnityEngine;

public class BottleEmptierGasConfig : IBuildingConfig
{
	public const string ID = "BottleEmptierGas";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("BottleEmptierGas", 1, 3, "gas_emptying_station_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.PENALTY.TIER2);
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
		storage.storageFilters = STORAGEFILTERS.GASES;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.capacityKg = 200f;
		go.AddOrGet<TreeFilterable>();
		BottleEmptier bottleEmptier = go.AddOrGet<BottleEmptier>();
		bottleEmptier.isGasEmptier = true;
		bottleEmptier.emptyRate = 0.25f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
