using TUNING;
using UnityEngine;

public class AdvancedResearchCenterConfig : IBuildingConfig
{
	public const string ID = "AdvancedResearchCenter";

	public const float BASE_RESEARCH_SPEED = 0.83f;

	public const float MASS_PER_POINT = 50f;

	public const float CAPACITY = 750f;

	public static readonly Tag INPUT_MATERIAL = GameTags.Water;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("AdvancedResearchCenter", 3, 3, "research_center2_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = INPUT_MATERIAL;
		manualDeliveryKG.refillMass = 150f;
		manualDeliveryKG.capacity = 750f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.ResearchFetch.IdHash;
		ResearchCenter researchCenter = go.AddOrGet<ResearchCenter>();
		researchCenter.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_research2_kanim")
		};
		researchCenter.research_point_type_id = "beta";
		researchCenter.inputMaterial = INPUT_MATERIAL;
		researchCenter.mass_per_point = 50f;
		researchCenter.requiredSkillPerk = Db.Get().SkillPerks.AllowAdvancedResearch.Id;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(INPUT_MATERIAL, 0.83f)
		};
		elementConverter.showDescriptors = false;
		go.AddOrGetDef<PoweredController.Def>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
