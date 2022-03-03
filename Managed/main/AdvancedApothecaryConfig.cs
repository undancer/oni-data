using TUNING;
using UnityEngine;

public class AdvancedApothecaryConfig : IBuildingConfig
{
	public const string ID = "AdvancedApothecary";

	public const float PARTICLE_CAPACITY = 400f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("AdvancedApothecary", 3, 3, "medicine_nuclear_kanim", 250, 240f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 2f;
		obj.UseHighEnergyParticleInputPort = true;
		obj.HighEnergyParticleInputOffset = new CellOffset(0, 2);
		obj.ViewMode = OverlayModes.Radiation.ID;
		obj.AudioCategory = "Glass";
		obj.AudioSize = "large";
		obj.Deprecated = true;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.capacity = 400f;
		highEnergyParticleStorage.showCapacityStatusItem = true;
		go.AddOrGet<HighEnergyParticlePort>().requireOperational = false;
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		AdvancedApothecary fabricator = go.AddOrGet<AdvancedApothecary>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
		go.AddOrGet<ComplexFabricatorWorkable>();
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ActiveParticleConsumer.Def def = go.AddOrGetDef<ActiveParticleConsumer.Def>();
		def.activeConsumptionRate = 1f;
		def.minParticlesForOperational = 1f;
		def.meterSymbolName = null;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredController.Def>();
	}
}
