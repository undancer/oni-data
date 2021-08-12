using TUNING;
using UnityEngine;

public class OrbitalResearchCenterConfig : IBuildingConfig
{
	public const string ID = "OrbitalResearchCenter";

	public const float BASE_SECONDS_PER_POINT = 100f;

	public const float MASS_PER_POINT = 5f;

	public const float BASE_MASS_PER_SECOND = 0.05f;

	public const float CAPACITY = 75f;

	public static readonly Tag INPUT_MATERIAL = SimHashes.Polypropylene.CreateTag();

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("OrbitalResearchCenter", 2, 3, "orbital_research_station_kanim", 30, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.ExhaustKilowattsWhenActive = 0.125f;
		obj.SelfHeatKilowattsWhenActive = 0.5f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.RocketInteriorBuilding);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = INPUT_MATERIAL;
		manualDeliveryKG.refillMass = 15f;
		manualDeliveryKG.capacity = 75f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.ResearchFetch.IdHash;
		ResearchCenter researchCenter = go.AddOrGet<ResearchCenter>();
		researchCenter.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_orbital_research_station_kanim") };
		researchCenter.research_point_type_id = "orbital";
		researchCenter.inputMaterial = INPUT_MATERIAL;
		researchCenter.mass_per_point = 5f;
		researchCenter.requiredSkillPerk = Db.Get().SkillPerks.AllowOrbitalResearch.Id;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(INPUT_MATERIAL, 0.05f)
		};
		elementConverter.showDescriptors = false;
		go.AddOrGet<InOrbitRequired>();
		go.AddOrGetDef<PoweredController.Def>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
