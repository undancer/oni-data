using TUNING;
using UnityEngine;

public class JetSuitLockerConfig : IBuildingConfig
{
	public const string ID = "JetSuitLocker";

	public const float O2_CAPACITY = 200f;

	public const float SUIT_CAPACITY = 200f;

	private ConduitPortInfo secondaryInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 1));

	public override BuildingDef CreateBuildingDef()
	{
		string[] rEFINED_METALS = MATERIALS.REFINED_METALS;
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(construction_mass: new float[1]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0]
		}, construction_materials: rEFINED_METALS, melting_point: 1600f, build_location_rule: BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, id: "JetSuitLocker", width: 2, height: 4, anim: "changingarea_jetsuit_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.BONUS.TIER1);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.PreventIdleTraversalPastBuilding = true;
		obj.InputConduitType = ConduitType.Gas;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "JetSuitLocker");
		return obj;
	}

	private void AttachPort(GameObject go)
	{
		go.AddComponent<ConduitSecondaryInput>().portInfo = secondaryInputPort;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<SuitLocker>().OutfitTags = new Tag[1]
		{
			GameTags.JetSuit
		};
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.capacityKG = 200f;
		go.AddComponent<JetSuitLocker>().portInfo = secondaryInputPort;
		go.AddOrGet<AnimTileable>().tags = new Tag[2]
		{
			new Tag("JetSuitLocker"),
			new Tag("JetSuitMarker")
		};
		go.AddOrGet<Storage>().capacityKg = 500f;
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		AttachPort(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		AttachPort(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
