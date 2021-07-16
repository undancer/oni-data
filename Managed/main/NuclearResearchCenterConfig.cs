using TUNING;
using UnityEngine;

public class NuclearResearchCenterConfig : IBuildingConfig
{
	public const string ID = "NuclearResearchCenter";

	public const float BASE_TIME_PER_POINT = 100f;

	public const float PARTICLES_PER_POINT = 10f;

	public const float CAPACITY = 100f;

	public static readonly Tag INPUT_MATERIAL = GameTags.HighEnergyParticle;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("NuclearResearchCenter", 5, 3, "material_research_centre_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.NONE);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.UseHighEnergyParticleInputPort = true;
		obj.HighEnergyParticleInputOffset = new CellOffset(-2, 1);
		obj.ViewMode = OverlayModes.Radiation.ID;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		obj.Deprecated = !Sim.IsRadiationEnabled();
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, "NuclearResearchCenter");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.capacity = 100f;
		NuclearResearchCenterWorkable nuclearResearchCenterWorkable = go.AddOrGet<NuclearResearchCenterWorkable>();
		nuclearResearchCenterWorkable.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_material_research_centre_kanim")
		};
		nuclearResearchCenterWorkable.requiredSkillPerk = Db.Get().SkillPerks.AllowNuclearResearch.Id;
		NuclearResearchCenter nuclearResearchCenter = go.AddOrGet<NuclearResearchCenter>();
		nuclearResearchCenter.researchTypeID = "nuclear";
		nuclearResearchCenter.materialPerPoint = 10f;
		nuclearResearchCenter.timePerPoint = 100f;
		nuclearResearchCenter.inputMaterial = INPUT_MATERIAL;
		go.AddOrGetDef<PoweredController.Def>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
