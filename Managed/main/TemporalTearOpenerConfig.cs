using TUNING;
using UnityEngine;

public class TemporalTearOpenerConfig : IBuildingConfig
{
	public const string ID = "TemporalTearOpener";

	public const float PARTICLES_CAPACITY = 10000f;

	public const float NUM_PARTICLES_TO_OPEN_TEAR = 6000f;

	public const float PARTICLE_CONSUME_RATE = 20f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("TemporalTearOpener", 5, 4, "temporal_tear_opener_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_METALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER6, decor: BUILDINGS.DECOR.BONUS.TIER2);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.Entombable = false;
		buildingDef.UseHighEnergyParticleInputPort = true;
		buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.capacity = 10000f;
		TemporalTearOpener.Def def = go.AddOrGetDef<TemporalTearOpener.Def>();
		def.numParticlesToOpen = 6000f;
		def.consumeRate = 20f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
