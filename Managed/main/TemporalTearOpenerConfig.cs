using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class TemporalTearOpenerConfig : IBuildingConfig
{
	public const string ID = "TemporalTearOpener";

	public const string PORT_ID = "HEP_STORAGE";

	public const float PARTICLES_CAPACITY = 1000f;

	public const float NUM_PARTICLES_TO_OPEN_TEAR = 10000f;

	public const float PARTICLE_CONSUME_RATE = 5f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("TemporalTearOpener", 3, 4, "temporal_tear_opener_kanim", 100, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_METALS, 2400f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER6, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER2);
		obj.DefaultAnimState = "off";
		obj.Entombable = false;
		obj.Invincible = true;
		obj.UseHighEnergyParticleInputPort = true;
		obj.HighEnergyParticleInputOffset = new CellOffset(0, 2);
		obj.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort("HEP_STORAGE", new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE, STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE, STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE) };
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.capacity = 1000f;
		highEnergyParticleStorage.PORT_ID = "HEP_STORAGE";
		highEnergyParticleStorage.showCapacityStatusItem = true;
		TemporalTearOpener.Def def = go.AddOrGetDef<TemporalTearOpener.Def>();
		def.numParticlesToOpen = 10000f;
		def.consumeRate = 5f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Deconstructable>().allowDeconstruction = false;
	}
}
