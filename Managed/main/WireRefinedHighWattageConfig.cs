using TUNING;
using UnityEngine;

public class WireRefinedHighWattageConfig : BaseWireConfig
{
	public const string ID = "WireRefinedHighWattage";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = CreateBuildingDef("WireRefinedHighWattage", "utilities_electric_conduct_hiwatt_kanim", 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, 0.05f, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER3);
		obj.MaterialCategory = MATERIALS.REFINED_METALS;
		obj.BuildLocationRule = BuildLocationRule.NotInTiles;
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		DoPostConfigureComplete(Wire.WattageRating.Max50000, go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.CanPowerTinker.Id;
	}
}
