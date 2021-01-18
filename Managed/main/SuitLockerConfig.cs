using TUNING;
using UnityEngine;

public class SuitLockerConfig : IBuildingConfig
{
	public const string ID = "SuitLocker";

	public override BuildingDef CreateBuildingDef()
	{
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, construction_materials: rEFINED_METALS, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, id: "SuitLocker", width: 1, height: 3, anim: "changingarea_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.BONUS.TIER1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.PreventIdleTraversalPastBuilding = true;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "SuitLocker");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		SuitLocker suitLocker = go.AddOrGet<SuitLocker>();
		suitLocker.OutfitTags = new Tag[1]
		{
			GameTags.AtmoSuit
		};
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.capacityKG = 200f;
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.tags = new Tag[2]
		{
			new Tag("SuitLocker"),
			new Tag("SuitMarker")
		};
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
