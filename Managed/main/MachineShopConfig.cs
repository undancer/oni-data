using TUNING;
using UnityEngine;

public class MachineShopConfig : IBuildingConfig
{
	public const string ID = "MachineShop";

	public static readonly Tag MATERIAL_FOR_TINKER = GameTags.RefinedMetal;

	public const float MASS_PER_TINKER = 5f;

	public static readonly string ROLE_PERK = "IncreaseMachinery";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MachineShop", 4, 2, "machineshop_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.NONE);
		buildingDef.Deprecated = true;
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.MachineShop);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
