using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class HighEnergyParticleSpawnerConfig : IBuildingConfig
{
	public const string ID = "HighEnergyParticleSpawner";

	public const float MIN_LAUNCH_INTERVAL = 2f;

	public const float RADIATION_SAMPLE_RATE = 0.2f;

	public const int MIN_SLIDER = 50;

	public const int MAX_SLIDER = 500;

	public const float DISABLED_CONSUMPTION_RATE = 1f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("HighEnergyParticleSpawner", 1, 2, "radiation_collector_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.NotInTiles, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER1);
		obj.Floodable = false;
		obj.AudioCategory = "Metal";
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.Radiation.ID;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.UseHighEnergyParticleOutputPort = true;
		obj.HighEnergyParticleOutputOffset = new CellOffset(0, 1);
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(0, 0);
		obj.EnergyConsumptionWhenActive = 480f;
		obj.ExhaustKilowattsWhenActive = 1f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(HighEnergyParticleSpawner.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLESPAWNER.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLESPAWNER.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.HIGHENERGYPARTICLESPAWNER.LOGIC_PORT_INACTIVE)
		};
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.RadiationIDs, "HighEnergyParticleSpawner");
		obj.Deprecated = !Sim.IsRadiationEnabled();
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		Prioritizable.AddRef(go);
		go.AddOrGet<HighEnergyParticleStorage>();
		go.AddOrGet<LoopingSounds>();
		HighEnergyParticleSpawner highEnergyParticleSpawner = go.AddOrGet<HighEnergyParticleSpawner>();
		highEnergyParticleSpawner.minLaunchInterval = 2f;
		highEnergyParticleSpawner.radiationSampleRate = 0.2f;
		highEnergyParticleSpawner.minSlider = 50;
		highEnergyParticleSpawner.maxSlider = 500;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
